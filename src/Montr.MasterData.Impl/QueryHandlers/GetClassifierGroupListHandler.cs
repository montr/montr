using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierGroupListHandler : IRequestHandler<GetClassifierGroupList, SearchResult<ClassifierGroup>>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IClassifierTreeService _classifierTreeService;

		public GetClassifierGroupListHandler(IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IClassifierTreeService classifierTreeService)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
			_classifierTreeService = classifierTreeService;
		}

		public async Task<SearchResult<ClassifierGroup>> Handle(GetClassifierGroupList request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				if (type.HierarchyType == HierarchyType.Groups)
				{
					var tree = request.TreeUid != null
						? await _classifierTreeService.GetClassifierTree(/*request.CompanyUid,*/ request.TypeCode, request.TreeUid.Value, cancellationToken)
						: await _classifierTreeService.GetClassifierTree(/*request.CompanyUid,*/ request.TypeCode, request.TreeCode, cancellationToken);

					if (request.FocusUid != null)
					{
						return await GetGroupsByFocus(db, type, tree, request, cancellationToken);
					}

					return await GetGroupsByParent(db, type, tree, request.ParentUid, request, true);
				}

				if (type.HierarchyType == HierarchyType.Items)
				{
					if (request.FocusUid != null)
					{
						return await GetItemsByFocus(db, type, request, cancellationToken);
					}

					return await GetItemsByParent(db, type, request.ParentUid, request, true);
				}
			}

			return null;
		}

		private static async Task<SearchResult<ClassifierGroup>> GetGroupsByFocus(DbContext db,
			ClassifierType type, ClassifierTree tree, ClassifierGroupSearchRequest request, CancellationToken cancellationToken)
		{
			// get all parent uids of focused item
			var path = await (
					from focus in db.GetTable<DbClassifierGroup>()
					join closureUp in db.GetTable<DbClassifierClosure>() on focus.Uid equals closureUp.ChildUid
					join item in db.GetTable<DbClassifierGroup>() on closureUp.ParentUid equals item.Uid
					where /*focus.TypeUid == type.Uid &&*/ focus.Uid == request.FocusUid
					orderby closureUp.Level descending
					select item.ParentUid)
				.ToListAsync(cancellationToken);

			SearchResult<ClassifierGroup> result = null;

			List<ClassifierGroup> currentLevel = null;

			foreach (var parentUid in path)
			{
				if (parentUid == request.ParentUid)
				{
					// found requested parent, init result and starting level
					result = new SearchResult<ClassifierGroup>
					{
						Rows = currentLevel = new List<ClassifierGroup>()
					};
				}

				// if current level is not already inited
				if (currentLevel == null) continue;

				// try to move to deeper level...
				if (parentUid.HasValue)
				{
					var parent = currentLevel.SingleOrDefault(x => x.Uid == parentUid);

					if (parent != null)
					{
						parent.Children = currentLevel = new List<ClassifierGroup>();
					}
				}

				// ... and load children
				var chldrn = await GetGroupsByParent(db, type, tree, parentUid, request, false);

				currentLevel.AddRange(chldrn.Rows);
			}

			return result;
		}

		private static async Task<SearchResult<ClassifierGroup>> GetGroupsByParent(DbContext db,
			ClassifierType type, ClassifierTree tree, Guid? parentUid, ClassifierGroupSearchRequest request, bool calculateTotalCount)
		{
			var query = from tree1 in db.GetTable<DbClassifierTree>()
				join item in db.GetTable<DbClassifierGroup>() on tree.Uid equals item.TreeUid
				where tree1.Uid == tree.Uid && tree1.TypeUid == type.Uid && item.ParentUid == parentUid
				select item;

			var data = query
				.Apply(request, x => x.Code)
				.Select(x => new ClassifierGroup
				{
					Uid = x.Uid,
					Code = x.Code,
					Name = x.Name,
					TreeUid = x.TreeUid,
					ParentUid = x.ParentUid
				})
				.ToList();

			var result = new SearchResult<ClassifierGroup> { Rows = data };

			if (calculateTotalCount)
			{
				result.TotalCount = await query.CountAsync();
			}

			if (request.ExpandSingleChild && data.Count == 1)
			{
				var singleChild = data[0];

				var children = await GetGroupsByParent(db, type, tree, singleChild.Uid, request, false);

				singleChild.Children = children.Rows;
			}

			return result;
		}

		private static async Task<SearchResult<ClassifierGroup>> GetItemsByFocus(DbContext db,
			ClassifierType type, ClassifierGroupSearchRequest request, CancellationToken cancellationToken)
		{
			// get all parent uids of focused item
			var path = await (
					from focus in db.GetTable<DbClassifier>()
					join closureUp in db.GetTable<DbClassifierClosure>() on focus.Uid equals closureUp.ChildUid
					join item in db.GetTable<DbClassifier>() on closureUp.ParentUid equals item.Uid
					where /*focus.TypeUid == type.Uid &&*/ focus.Uid == request.FocusUid
					orderby closureUp.Level descending
					select item.ParentUid)
				.ToListAsync(cancellationToken);

			SearchResult<ClassifierGroup> result = null;

			List<ClassifierGroup> currentLevel = null;

			foreach (var parentUid in path)
			{
				if (parentUid == request.ParentUid)
				{
					// found requested parent, init result and starting level
					result = new SearchResult<ClassifierGroup>
					{
						Rows = currentLevel = new List<ClassifierGroup>()
					};
				}

				// if current level is not already inited
				if (currentLevel == null) continue;

				// try to move to deeper level...
				if (parentUid.HasValue)
				{
					var parent = currentLevel.SingleOrDefault(x => x.Uid == parentUid);

					if (parent != null)
					{
						parent.Children = currentLevel = new List<ClassifierGroup>();
					}
				}

				// ... and load children
				var chldrn = await GetItemsByParent(db, type, parentUid, request, false);

				currentLevel.AddRange(chldrn.Rows);
			}

			return result;
		}

		private static async Task<SearchResult<ClassifierGroup>> GetItemsByParent(DbContext db,
			ClassifierType type, Guid? parentUid, ClassifierGroupSearchRequest request, bool calculateTotalCount)
		{
			var query = from item in db.GetTable<DbClassifier>()
				where item.TypeUid == type.Uid && item.ParentUid == parentUid
				// orderby item.StatusCode, item.Code, item.Name
				select item;

			var data = query
				.Apply(request, x => x.Code)
				.Select(x => new ClassifierGroup
				{
					Uid = x.Uid,
					Code = x.Code,
					Name = x.Name,
					ParentUid = x.ParentUid
				})
				.ToList();

			var result = new SearchResult<ClassifierGroup> { Rows = data };

			if (calculateTotalCount)
			{
				result.TotalCount = await query.CountAsync();
			}

			if (request.ExpandSingleChild && result.Rows.Count == 1)
			{
				var singleChild = data[0];

				var children = await GetItemsByParent(db, type, singleChild.Uid, request, false);

				singleChild.Children = children.Rows;
			}

			return result;
		}
	}
}
