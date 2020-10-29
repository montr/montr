using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;
using Montr.MasterData.Commands;

namespace Montr.MasterData.Services
{
	// todo: convert to ClassifierTypeRegistrator service
	public abstract class AbstractRegisterClassifierTypeStartupTask : IStartupTask
	{
		private readonly ILogger _logger;
		private readonly IMediator _mediator;

		protected AbstractRegisterClassifierTypeStartupTask(ILogger logger, IMediator mediator)
		{
			_logger = logger;
			_mediator = mediator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			foreach (var command in GetCommands())
			{
				var result = await _mediator.Send(command, cancellationToken);

				result.AssertSuccess(() => $"Failed to register classifier type \"{command.Item.Code}\"");

				if (result.AffectedRows == 1)
				{
					_logger.LogInformation("Classifier type {code} successfully registered.", command.Item.Code);
				}
				else
				{
					_logger.LogDebug("Classifier type {code} already registered.", command.Item.Code);
				}
			}
		}

		protected abstract IEnumerable<RegisterClassifierType> GetCommands();
	}
}
