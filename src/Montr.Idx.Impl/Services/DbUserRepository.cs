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
	public class DbUserRepository : DbClassifierRepository<User>
	{
		private readonly IUserManager _userManager;

		public DbUserRepository(
			IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService,
			IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService,
			IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator,
			IUserManager userManager)
			: base(
				dbContextFactory,
				classifierTypeService,
				classifierTreeService,
				metadataService,
				fieldDataRepository,
				numberGenerator)
		{
			_userManager = userManager;
		}

		public override async Task<ApiResult> Insert(Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.Insert(item, cancellationToken);

			if (result.Success)
			{
				return await _userManager.Create((User) item, cancellationToken);
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
				var role = await _userManager.Get(item.Uid.Value, cancellationToken);

				return await _userManager.Update(role, cancellationToken);
			}

			return result;
		}

		public override async Task<ApiResult> Delete(DeleteClassifier request, CancellationToken cancellationToken)
		{
			foreach (var uid in request.Uids)
			{
				var role = await _userManager.Get(uid, cancellationToken);

				var result = await _userManager.Delete(role, cancellationToken);

				if (result.Success == false) return result;
			}

			return await base.Delete(request, cancellationToken);
		}
	}
}
