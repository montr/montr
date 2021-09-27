using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Commands;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Models;

namespace Montr.Automate.Impl.CommandHandlers
{
	public class RunAutomationsHandler : IRequestHandler<RunAutomations, ApiResult>
	{
		// private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IAutomationRunner _automationRunner;

		public RunAutomationsHandler(/*, IUnitOfWorkFactory unitOfWorkFactory,*/ IAutomationRunner automationRunner)
		{
			// _unitOfWorkFactory = unitOfWorkFactory;
			_automationRunner = automationRunner;
		}

		public async Task<ApiResult> Handle(RunAutomations request, CancellationToken cancellationToken)
		{
			// using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _automationRunner.Run(new AutomationContext
				{
					EntityTypeCode = request.EntityTypeCode,
					EntityTypeUid = request.EntityTypeUid,
					EntityUid = request.EntityUid
				}, cancellationToken);

				// scope.Commit();

				return result;
			}
		}
	}
}
