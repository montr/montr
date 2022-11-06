using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.Implementations;
using Montr.Metadata.Services;
using Montr.Tasks.Impl.Entities;
using Montr.Tasks.Models;

namespace Montr.Tasks.Impl.Services
{
	public class DbTaskTypeRepository : DbClassifierRepository<TaskType>
	{
		public DbTaskTypeRepository(IDbContextFactory dbContextFactory,
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
				await db.GetTable<DbTaskType>()
					.Value(x => x.Uid, item.Uid)
					.InsertAsync(cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> UpdateInternal(DbContext db,
			ClassifierType type, ClassifierTree tree, Classifier item, CancellationToken cancellationToken = default)
		{
			var result = await base.UpdateInternal(db, type, tree, item, cancellationToken);

			return result;
		}

		protected override async Task<ApiResult> DeleteInternal(DbContext db,
			ClassifierType type, DeleteClassifier request, CancellationToken cancellationToken = default)
		{
			await db.GetTable<DbNumerator>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			await db.GetTable<DbTaskType>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			return await base.DeleteInternal(db, type, request, cancellationToken);
		}
	}
}
