using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;
using Montr.Messages.Commands;

namespace Montr.Messages.Services
{
	public abstract class AbstractRegisterMessageTemplateStartupTask : IStartupTask
	{
		private readonly ILogger _logger;
		private readonly IMediator _mediator;

		protected AbstractRegisterMessageTemplateStartupTask(ILogger logger, IMediator mediator)
		{
			_logger = logger;
			_mediator = mediator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			foreach (var command in GetCommands())
			{
				var result = await _mediator.Send(command, cancellationToken);

				result.AssertSuccess(() => $"Failed to register message template \"{command.Item.Uid}\"");

				if (result.AffectedRows == 1)
				{
					_logger.LogInformation("Message template {code} successfully registered.", command.Item.Uid);
				}
				else
				{
					_logger.LogDebug("Message template {code} already registered.", command.Item.Uid);
				}
			}
		}

		protected abstract IEnumerable<RegisterMessageTemplate> GetCommands();
	}
}
