﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Entities;
using Montr.Docs.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.Implementations;
using Montr.Metadata.Services;

namespace Montr.Docs.Services.Implementations
{
	public class DbDocumentTypeRepository : DbClassifierRepository<DocumentType>
	{
		public DbDocumentTypeRepository(IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository, INumberGenerator numberGenerator)
			: base(dbContextFactory, classifierTypeService, classifierTreeService, metadataService, fieldDataRepository, numberGenerator)
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

			await db.GetTable<DbDocumentType>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			return await base.DeleteInternal(db, type, request, cancellationToken);
		}
	}
}
