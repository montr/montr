using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Commands;
using Montr.Automate.Models;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Automate.Services.CommandHandlers
{
	/// <summary>
	/// Typically executes as background task.
	/// </summary>
	public class RunAutomationsHandler : IRequestHandler<RunAutomations, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IAutomationRunner _automationRunner;

		public RunAutomationsHandler(IUnitOfWorkFactory unitOfWorkFactory, IAutomationRunner automationRunner)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_automationRunner = automationRunner;
		}

		public async Task<ApiResult> Handle(RunAutomations request, CancellationToken cancellationToken)
		{
			// Automations should be run in transaction to prevent db trails
			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _automationRunner.Run(new AutomationContext
				{
					EntityTypeCode = request.EntityTypeCode,
					EntityUid = request.EntityUid
				}, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
