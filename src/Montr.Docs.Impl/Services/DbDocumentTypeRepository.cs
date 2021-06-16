using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Impl.Entities;
using Montr.Docs.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Services;

namespace Montr.Docs.Impl.Services
{
	public class DbDocumentTypeRepository2 : DbClassifierRepository<DocumentType>
	{
		public DbDocumentTypeRepository2(IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator)
			: base(dbContextFactory, classifierTypeService, classifierTreeService, metadataService, fieldDataRepository,
				numberGenerator)
		{
		}

		protected override async Task<ApiResult> InsertInternal(DbContext db,
			ClassifierType type, Classifier item, CancellationToken cancellationToken = default)
		{
			var result = await base.InsertInternal(db, type, item, cancellationToken);

			if (result.Success)
			{
				// var numerator = (Numerator)item;

				await db.GetTable<DbDocumentType>()
					.Value(x => x.Uid, item.Uid)
					.InsertAsync(cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> UpdateInternal(DbContext db,
			ClassifierType type, ClassifierTree tree, Classifier item, CancellationToken cancellationToken = default)
		{
			var result = await base.UpdateInternal(db, type, tree, item, cancellationToken);

			/*if (result.Success)
			{
				var numerator = (Numerator)item;

				await db.GetTable<DbDocumentType>()
					.Where(x => x.Uid == item.Uid)
					.Set(x => x.Pattern, numerator.Pattern)
					.UpdateAsync(cancellationToken);
			}*/

			return result;
		}

		protected override async Task<ApiResult> DeleteInternal(DbContext db,
			ClassifierType type, DeleteClassifier request, CancellationToken cancellationToken = default)
		{
			await db.GetTable<DbNumerator>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			return await base.DeleteInternal(db, type, request, cancellationToken);
		}
	}

	public class DbDocumentTypeRepository : IRepository<DocumentType>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbDocumentTypeRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<DocumentType>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (DocumentTypeSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = db.GetTable<DbDocumentType>().AsQueryable();

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				if (request.Code != null)
				{
					query = query.Where(x => x.Code == request.Code);
				}

				var data = await Materialize(
					query.Apply(request, x => x.Code, SortOrder.Descending), cancellationToken);

				return new SearchResult<DocumentType>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static async Task<List<DocumentType>> Materialize(IQueryable<DbDocumentType> query, CancellationToken cancellationToken)
		{
			return await query.Select(x => new DocumentType
			{
				Uid = x.Uid,
				Code = x.Code,
				Name = x.Name,
				Url = "/documentTypes/view/" + x.Uid
			}).ToListAsync(cancellationToken);
		}
	}
}
