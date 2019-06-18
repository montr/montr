using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbClassifierRepository : IRepository<Classifier>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public DbClassifierRepository(IDbContextFactory dbContextFactory, IClassifierTypeService classifierTypeService)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<SearchResult<Classifier>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				IQueryable<DbClassifier> query = null;

				if (type.HierarchyType == HierarchyType.None /* || request.GroupUid == null  || request.GroupCode == "."  wtf? */)
				{
					// no-op
				}
				else if (type.HierarchyType == HierarchyType.Groups)
				{
					// todo: send only selected group uid
					var groupUid = request.GroupUid ?? request.TreeUid;

					if (groupUid != null)
					{
						// show only selected group children
						if (request.Depth == null || request.Depth == "0")
						{
							query = from types in db.GetTable<DbClassifierType>()
								// join trees in db.GetTable<DbClassifierTree>() on types.Uid equals trees.TypeUid
								join children_groups in db.GetTable<DbClassifierGroup>() on types.Uid equals children_groups.TypeUid
								join links in db.GetTable<DbClassifierLink>() on children_groups.Uid equals links.GroupUid
								join c in db.GetTable<DbClassifier>() on links.ItemUid equals c.Uid
								where types.CompanyUid == request.CompanyUid &&
									types.Code == request.TypeCode &&
									// trees.Code == request.TreeCode &&
									children_groups.Uid == groupUid
									select c;
						}
						else
						{
							query = from types in db.GetTable<DbClassifierType>()
								// join trees in db.GetTable<DbClassifierTree>() on types.Uid equals trees.TypeUid
								join parent_groups in db.GetTable<DbClassifierGroup>() on types.Uid equals parent_groups.TypeUid
								join closures in db.GetTable<DbClassifierClosure>() on parent_groups.Uid equals closures.ParentUid
								join children_groups in db.GetTable<DbClassifierGroup>() on closures.ChildUid equals children_groups.Uid
								join links in db.GetTable<DbClassifierLink>() on children_groups.Uid equals links.GroupUid
								join c in db.GetTable<DbClassifier>() on links.ItemUid equals c.Uid
								where types.CompanyUid == request.CompanyUid &&
									types.Code == request.TypeCode &&
									// trees.Code == request.TreeCode &&
									parent_groups.Uid == groupUid
									select c;
						}
					}
				}
				else if (type.HierarchyType == HierarchyType.Items)
				{
					if (request.Depth == null || request.Depth == "0")
					{
						query = from parent in db.GetTable<DbClassifier>()
							join @class in db.GetTable<DbClassifier>() on parent.Uid equals @class.ParentUid
							where parent.TypeUid == type.Uid && parent.Uid == request.GroupUid
								select @class;
					}
					else
					{
						query = from parent in db.GetTable<DbClassifier>()
							join closures in db.GetTable<DbClassifierClosure>() on parent.Uid equals closures.ParentUid
							join @class in db.GetTable<DbClassifier>() on closures.ChildUid equals @class.Uid
							where parent.TypeUid == type.Uid && parent.Uid == request.GroupUid && closures.Level > 0
							select @class;
					}
				}

				if (query == null)
				{
					query = from types in db.GetTable<DbClassifierType>()
						join c in db.GetTable<DbClassifier>() on types.Uid equals c.TypeUid
						where types.CompanyUid == request.CompanyUid &&
							types.Code == request.TypeCode
						select c;
				}

				if (request.Uid.HasValue)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				var data = await query
					.Apply(request, x => x.Code)
					.Select(x => new Classifier
					{
						Uid = x.Uid,
						StatusCode = x.StatusCode,
						Code = x.Code,
						Name = x.Name,
						Url = $"/classifiers/{type.Code}/edit/{x.Uid}"
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<Classifier>
				{
					TotalCount = query.Count(),
					Rows = data
				};
			}
		}
	}
}
