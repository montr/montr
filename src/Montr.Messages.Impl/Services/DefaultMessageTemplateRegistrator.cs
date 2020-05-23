using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Messages.Models;
using Montr.Messages.Services;

namespace Montr.Messages.Impl.Services
{
	public class DefaultMessageTemplateRegistrator : IMessageTemplateRegistrator
	{
		private readonly ILogger<DefaultMessageTemplateRegistrator> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IMessageTemplateService _messageTemplateService;

		public DefaultMessageTemplateRegistrator(
			ILogger<DefaultMessageTemplateRegistrator> logger,
			IUnitOfWorkFactory unitOfWorkFactory,
			IMessageTemplateService messageTemplateService)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_messageTemplateService = messageTemplateService;
		}

		public async Task<ApiResult> Register(MessageTemplate item, CancellationToken cancellationToken)
		{
			// todo: use codes
			var type = await _messageTemplateService.TryGet(item.Uid, cancellationToken);

			if (type != null)
			{
				_logger.LogDebug("Message template {code} already registered.", item.Uid);

				return new ApiResult { Success = false };
			}

			using (var scope = _unitOfWorkFactory.Create())
			{
				var insertTypeResult = await _messageTemplateService.Insert(item, cancellationToken);

				insertTypeResult.AssertSuccess(() => $"Failed to register message template \"{item.Uid}\"");

				scope.Commit();

				_logger.LogInformation("Message template {code} successfully registered.", item.Uid);

				return insertTypeResult;
			}
		}
	}
}
