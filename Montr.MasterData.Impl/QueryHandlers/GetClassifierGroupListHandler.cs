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

			if (type.HierarchyType == HierarchyType.Groups)
			{
				using (var db = _dbContextFactory.Create())
				{
					IQueryable<DbClassifierGroup> query;

					if (request.FocusUid != null)
					{
						query = from tree in db.GetTable<DbClassifierTree>()
							join focus in db.GetTable<DbClassifierGroup>() on tree.Uid equals focus.TreeUid
							join closureUp in db.GetTable<DbClassifierClosure>() on focus.Uid equals closureUp.ChildUid
							join closureDown in db.GetTable<DbClassifierClosure>() on closureUp.ParentUid equals closureDown.ParentUid
							join result in db.GetTable<DbClassifierGroup>() on closureDown.ChildUid equals result.Uid
							where tree.TypeUid == type.Uid
								&& tree.Code == request.TreeCode
								&& focus.Uid == request.FocusUid
								&& closureUp.Level > 0
							select result;

						var children = Materialize(query);
						var roots = GetGroupsByParent(db, type, request.TreeCode, null);

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

						return roots;
					}

					return GetGroupsByParent(db, type, request.TreeCode, request.ParentCode);
				}
			}

			if (type.HierarchyType == HierarchyType.Items)
			{
				using (var db = _dbContextFactory.Create())
				{
					IQueryable<DbClassifier> query;

					if (request.ParentCode == null)
					{
						query = from item in db.GetTable<DbClassifier>()
							where item.TypeUid == type.Uid
								&& item.ParentUid == null
							orderby item.StatusCode, item.Code, item.Name
							select item;
					}
					else
					{
						query = from item in db.GetTable<DbClassifier>()
							join parent in db.GetTable<DbClassifier>() on item.ParentUid equals parent.Uid
							where item.TypeUid == type.Uid
								&& parent.Code == request.ParentCode
							select item;
					}

					return query
						.OrderBy(x => x.StatusCode)
						.ThenBy(x => x.Code)
						.ThenBy(x => x.Name)
						.Take(Paging.MaxPageSize)
						.Select(x => new ClassifierGroup
						{
							Code = x.Code,
							Name = x.Name
						})
						.ToList();
				}
			}

			return ImmutableList<ClassifierGroup>.Empty;
		}

		private static IList<ClassifierGroup> GetGroupsByParent(DbContext db, 	ClassifierType type, string treeCode, string parentCode)
		{
			IQueryable<DbClassifierGroup> query;

			if (parentCode != null)
			{
				query = from tree in db.GetTable<DbClassifierTree>()
					join item in db.GetTable<DbClassifierGroup>() on tree.Uid equals item.TreeUid
					join parent in db.GetTable<DbClassifierGroup>() on item.ParentUid equals parent.Uid
					where tree.TypeUid == type.Uid
						&& tree.Code == treeCode
						&& parent.Code == parentCode
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

			return Materialize(query);
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
	}
}
