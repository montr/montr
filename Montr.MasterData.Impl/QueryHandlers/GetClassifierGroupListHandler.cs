using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
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

		public GetClassifierGroupListHandler(IDbContextFactory dbContextFactory, IClassifierTypeService classifierTypeService)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<SearchResult<ClassifierGroup>> Handle(GetClassifierGroupList command, CancellationToken cancellationToken)
		{
			var request = command.Request ?? throw new ArgumentNullException(nameof(command.Request));

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				if (type.HierarchyType == HierarchyType.Groups)
				{
					if (request.FocusUid != null)
					{
						return await GetGroupsByFocus(db, type, request, cancellationToken);
					}

					return await GetGroupsByParent(db, type, request.ParentUid, request, true);
				}
				
				if (type.HierarchyType == HierarchyType.Items)
				{
					return await GetItemsByParent(db, type, request.ParentUid, request, true);
				}
			}

			return null;
		}

		private static async Task<SearchResult<ClassifierGroup>> GetGroupsByFocus(DbContext db,
			ClassifierType type, ClassifierGroupSearchRequest request, CancellationToken cancellationToken)
		{
			var path = await (
					from focus in db.GetTable<DbClassifierGroup>()
					join closureUp in db.GetTable<DbClassifierClosure>() on focus.Uid equals closureUp.ChildUid
					join item in db.GetTable<DbClassifierGroup>() on closureUp.ParentUid equals item.Uid
					where focus.TypeUid == type.Uid && focus.Uid == request.FocusUid
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
				var chldrn = await GetGroupsByParent(db, type, parentUid, request, false);

				currentLevel.AddRange(chldrn.Rows);
			}

			return result;
		}

		private static async Task<SearchResult<ClassifierGroup>> GetGroupsByParent(DbContext db,
			ClassifierType type, Guid? parentUid, ClassifierGroupSearchRequest request, bool calculateTotalCount)
		{
			var query = from item in db.GetTable<DbClassifierGroup>()
				where item.TypeUid == type.Uid && item.ParentUid == parentUid
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

			if (request.ExpandSingleChild && data.Count == 1)
			{
				var singleChild = data[0];

				var children = await GetGroupsByParent(db, type, singleChild.Uid, request, false);

				singleChild.Children = children.Rows;
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
