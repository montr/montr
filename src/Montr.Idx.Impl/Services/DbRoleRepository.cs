using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Data.Linq2Db;
using Montr.Idx.Models;
using Montr.Idx.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Services;

namespace Montr.Idx.Impl.Services
{
	public class DbRoleRepository : DbClassifierRepository<Role>
	{
		private readonly IRoleManager _roleManager;

		public DbRoleRepository(
			IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService,
			IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService,
			IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator,
			IRoleManager roleManager)
			: base(
				dbContextFactory,
				classifierTypeService,
				classifierTreeService,
				metadataService,
				fieldDataRepository,
				numberGenerator)
		{
			_roleManager = roleManager;
		}

		public override async Task<ApiResult> Insert(Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.Insert(item, cancellationToken);

			if (result.Success)
			{
				return await _roleManager.Create((Role) item, cancellationToken);
			}

			return result;
		}

		public override async Task<ApiResult> Update(Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.Update(item, cancellationToken);

			if (result.Success)
			{
				// todo: restore optimistic concurrency check (?)
				// ReSharper disable once PossibleInvalidOperationException
				var role = await _roleManager.Get(item.Uid.Value, cancellationToken);

				return await _roleManager.Update(role, cancellationToken);
			}

			return result;
		}

		public override async Task<ApiResult> Delete(DeleteClassifier request, CancellationToken cancellationToken)
		{
			foreach (var uid in request.Uids)
			{
				var role = await _roleManager.Get(uid, cancellationToken);

				var result = await _roleManager.Delete(role, cancellationToken);

				if (result.Success == false) return result;
			}

			return await base.Delete(request, cancellationToken);
		}
	}
}
