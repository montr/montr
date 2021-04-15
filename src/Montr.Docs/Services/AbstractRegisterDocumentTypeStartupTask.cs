using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;
using Montr.Docs.Commands;

namespace Montr.Docs.Services
{
	public abstract class AbstractRegisterDocumentTypeStartupTask : IStartupTask
	{
		private readonly ILogger _logger;
		private readonly ISender _mediator;

		protected AbstractRegisterDocumentTypeStartupTask(ILogger logger, ISender mediator)
		{
			_logger = logger;
			_mediator = mediator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			foreach (var command in GetCommands())
			{
				var result = await _mediator.Send(command, cancellationToken);

				result.AssertSuccess(() => $"Failed to register document type \"{command.Item.Code}\"");

				if (result.AffectedRows == 1)
				{
					_logger.LogInformation("Document type {code} successfully registered.", command.Item.Code);
				}
				else
				{
					_logger.LogDebug("Document type {code} already registered.", command.Item.Code);
				}
			}
		}

		protected abstract IEnumerable<RegisterDocumentType> GetCommands();
	}
}
