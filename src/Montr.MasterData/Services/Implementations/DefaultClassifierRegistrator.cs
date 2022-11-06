using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.Implementations
{
	public class DefaultClassifierRegistrator : IClassifierRegistrator
	{
		private readonly ILogger<DefaultClassifierRegistrator> _logger;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;

		public DefaultClassifierRegistrator(
			ILogger<DefaultClassifierRegistrator> logger,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory)
		{
			_logger = logger;
			_classifierRepositoryFactory = classifierRepositoryFactory;
		}

		public async Task<ApiResult> Register(Classifier item, CancellationToken cancellationToken)
		{
			var repository = _classifierRepositoryFactory.GetNamedOrDefaultService(item.Type);

			var existing = await FindExisting(repository, item, cancellationToken);

			if (existing != null)
			{
				_logger.LogDebug("Classifier {classifier} already registered.", existing);

				return new ApiResult { Success = false };
			}

			var result = await repository.Insert(item, cancellationToken);

			result.AssertSuccess(() => $"Failed to register classifier {item}");

			_logger.LogInformation("Classifier {item} successfully registered.", item);

			return result;
		}

		private static async Task<Classifier> FindExisting(IClassifierRepository repository, Classifier item, CancellationToken cancellationToken)
		{
			if (item.Uid.HasValue)
			{
				return await repository.Get(item.Type, item.Uid.Value, cancellationToken);
			}

			if (item.Code != null)
			{
				return await repository.Get(item.Type, item.Code, cancellationToken);
			}

			return null;
		}
	}
}
