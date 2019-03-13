using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.Services
{
	public class DbClassifierRepository : IRepository<Classifier>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<ClassifierType> _classifierTypeRepository;

		public DbClassifierRepository(IDbContextFactory dbContextFactory, IRepository<ClassifierType> classifierTypeRepository)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeRepository = classifierTypeRepository;
		}

		public async Task<SearchResult<Classifier>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierSearchRequest)searchRequest;

			var typez = await _classifierTypeRepository.Search(
				new ClassifierTypeSearchRequest
				{
					CompanyUid = request.CompanyUid,
					Code = request.TypeCode
				}, cancellationToken);

			var type = typez.Rows.Single();

			using (var db = _dbContextFactory.Create())
			{
				IQueryable<DbClassifier> all;

				if (type.HierarchyType == HierarchyType.Groups && request.GroupCode != null && request.GroupCode != ".")
				{
					if (request.Depth == null || request.Depth == "0")
					{
						all = from types in db.GetTable<DbClassifierType>()
							join trees in db.GetTable<DbClassifierTree>() on types.Uid equals trees.TypeUid
							join children_groups in db.GetTable<DbClassifierGroup>() on trees.Uid equals children_groups.TreeUid
							join links in db.GetTable<DbClassifierLink>() on children_groups.Uid equals links.GroupUid
							join c in db.GetTable<DbClassifier>() on links.ItemUid equals c.Uid
							where types.CompanyUid == request.CompanyUid &&
								types.Code == request.TypeCode &&
								trees.Code == request.TreeCode &&
								children_groups.Code == request.GroupCode
							select c;
					}
					else
					{
						all = from types in db.GetTable<DbClassifierType>()
							join trees in db.GetTable<DbClassifierTree>() on types.Uid equals trees.TypeUid
							join parent_groups in db.GetTable<DbClassifierGroup>() on trees.Uid equals parent_groups.TreeUid
							join closures in db.GetTable<DbClassifierClosure>() on parent_groups.Uid equals closures.ParentUid
							join children_groups in db.GetTable<DbClassifierGroup>() on closures.ChildUid equals children_groups.Uid
							join links in db.GetTable<DbClassifierLink>() on children_groups.Uid equals links.GroupUid
							join c in db.GetTable<DbClassifier>() on links.ItemUid equals c.Uid
							where types.CompanyUid == request.CompanyUid &&
								types.Code == request.TypeCode &&
								trees.Code == request.TreeCode &&
								parent_groups.Code == request.GroupCode
							select c;
					}
				}
				else
				{
					all = from types in db.GetTable<DbClassifierType>()
						join c in db.GetTable<DbClassifier>() on types.Uid equals c.TypeUid
						where types.CompanyUid == request.CompanyUid &&
							types.Code == request.TypeCode
						select c;
				}

				var data = await all
					.Apply(request, x => x.Code)
					.Select(x => new Classifier
					{
						Uid = x.Uid,
						StatusCode = x.StatusCode,
						Code = x.Code,
						Name = x.Name,
						// Url = $"/classifiers/{x.ConfigCode}/edit/{x.Uid}"
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<Classifier>
				{
					TotalCount = all.Count(),
					Rows = data
				};
			}
		}
	}
}
