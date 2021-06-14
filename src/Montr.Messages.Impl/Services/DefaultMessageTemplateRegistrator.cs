using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Messages.Models;
using Montr.Messages.Services;

namespace Montr.Messages.Impl.Services
{
	public class DefaultMessageTemplateRegistrator : IMessageTemplateRegistrator
	{
		private readonly ILogger<DefaultMessageTemplateRegistrator> _logger;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;

		public DefaultMessageTemplateRegistrator(
			ILogger<DefaultMessageTemplateRegistrator> logger,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory)
		{
			_logger = logger;
			_classifierRepositoryFactory = classifierRepositoryFactory;
		}

		public async Task<ApiResult> Register(MessageTemplate item, CancellationToken cancellationToken)
		{
			var repository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.MessageTemplate);

			// todo: use codes (?)
			var template = item.Uid.HasValue
				? (MessageTemplate)await repository.Get(ClassifierTypeCode.MessageTemplate, item.Uid.Value, cancellationToken)
				: null;

			if (template != null)
			{
				_logger.LogDebug("Message template {code} already registered.", item.Uid);

				return new ApiResult { Success = false };
			}

			var result = await repository.Insert(item, cancellationToken);

			result.AssertSuccess(() => $"Failed to register message template \"{item.Uid}\"");

			_logger.LogInformation("Message template {code} successfully registered.", item.Uid);

			return result;
		}
	}
}
