using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierGroupListHandler : IRequestHandler<GetClassifierGroupList, IList<ClassifierGroup>>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public GetClassifierGroupListHandler(IDbContextFactory dbContextFactory, IClassifierTypeService classifierTypeService)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<IList<ClassifierGroup>> Handle(GetClassifierGroupList command, CancellationToken cancellationToken)
		{
			var request = command.Request ?? throw new ArgumentNullException(nameof(command.Request));

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				if (type.HierarchyType == HierarchyType.Groups)
				{
					if (request.FocusUid != null)
					{
						var query = from tree in db.GetTable<DbClassifierTree>()
							join focus in db.GetTable<DbClassifierGroup>() on tree.Uid equals focus.TreeUid
							join closureUp in db.GetTable<DbClassifierClosure>() on focus.Uid equals closureUp.ChildUid
							join closureDown in db.GetTable<DbClassifierClosure>() on closureUp.ParentUid equals closureDown.ParentUid
							join result in db.GetTable<DbClassifierGroup>() on closureDown.ChildUid equals result.Uid
							where tree.TypeUid == type.Uid
								&& tree.Code == request.TreeCode
								&& focus.Uid == request.FocusUid
								&& closureUp.Level > 0 // to exclude just focused group
								&& (closureDown.Level == 0 && result.ParentUid == null ||
									closureDown.Level == 1 && result.ParentUid != null)
							select result;

						var children = Materialize(query);

						var roots = GetGroupsByParent(db, type, request.TreeCode, null);

						LinkChildrenToRoots(children, roots);

						return roots;
					}

					return GetGroupsByParent(db, type, request.TreeCode, request.ParentUid);
				}
				
				if (type.HierarchyType == HierarchyType.Items)
				{
					return GetItemsByParent(db, type, request.ParentUid);
				}
			}

			return ImmutableList<ClassifierGroup>.Empty;
		}

		private static void LinkChildrenToRoots(IList<ClassifierGroup> children, IList<ClassifierGroup> roots)
		{
			var map = children.ToDictionary(x => x.Uid);

			foreach (var child in children)
			{
				if (child.ParentUid.HasValue)
				{
					var parent = map[child.ParentUid.Value];

					if (parent.Children == null)
					{
						parent.Children = new List<ClassifierGroup>();
					}

					parent.Children.Add(child);
				}
				else
				{
					if (child.Children == null)
					{
						var root = roots.Single(x => x.Uid == child.Uid);

						root.Children = child.Children = new List<ClassifierGroup>();
					}
				}
			}
		}

		private static IList<ClassifierGroup> GetItemsByParent(DbContext db, ClassifierType type, Guid? parentUid)
		{
			IQueryable<DbClassifier> query;

			if (parentUid != null)
			{
				query = from item in db.GetTable<DbClassifier>()
					join parent in db.GetTable<DbClassifier>() on item.ParentUid equals parent.Uid
					where item.TypeUid == type.Uid
						&& parent.Uid == parentUid
					select item;
			}
			else
			{
				query = from item in db.GetTable<DbClassifier>()
					where item.TypeUid == type.Uid
						&& item.ParentUid == null
					orderby item.StatusCode, item.Code, item.Name
					select item;
			}

			var result = Materialize(query);

			if (result.Count == 1)
			{
				result[0].Children = GetItemsByParent(db, type, result[0].Uid);
			}

			return result;
		}

		private static IList<ClassifierGroup> GetGroupsByParent(DbContext db, ClassifierType type, string treeCode, Guid? parentUid)
		{
			IQueryable<DbClassifierGroup> query;

			if (parentUid != null)
			{
				query = from tree in db.GetTable<DbClassifierTree>()
					join item in db.GetTable<DbClassifierGroup>() on tree.Uid equals item.TreeUid
					join parent in db.GetTable<DbClassifierGroup>() on item.ParentUid equals parent.Uid
					where tree.TypeUid == type.Uid
						&& tree.Code == treeCode
						&& parent.Uid == parentUid
					select item;
			}
			else
			{
				query = from tree in db.GetTable<DbClassifierTree>()
					join item in db.GetTable<DbClassifierGroup>() on tree.Uid equals item.TreeUid
					where tree.TypeUid == type.Uid
						&& tree.Code == treeCode
						&& item.ParentUid == null
					select item;
			}

			var result = Materialize(query);

			if (result.Count == 1)
			{
				result[0].Children = GetGroupsByParent(db, type, treeCode, result[0].Uid);
			}

			return result;
		}

		private static IList<ClassifierGroup> Materialize(IQueryable<DbClassifierGroup> query)
		{
			return query
				.OrderBy(x => x.Code)
				.ThenBy(x => x.Name)
				.Take(Paging.MaxPageSize)
				.Select(x => new ClassifierGroup
				{
					Uid = x.Uid,
					Code = x.Code,
					Name = x.Name,
					ParentUid = x.ParentUid
				})
				.ToList();
		}

		private static IList<ClassifierGroup> Materialize(IQueryable<DbClassifier> query)
		{
			return query
				.OrderBy(x => x.StatusCode)
				.ThenBy(x => x.Code)
				.ThenBy(x => x.Name)
				.Take(Paging.MaxPageSize)
				.Select(x => new ClassifierGroup
				{
					Uid = x.Uid,
					Code = x.Code,
					Name = x.Name,
					ParentUid = x.ParentUid
				})
				.ToList();
		}
	}
}
