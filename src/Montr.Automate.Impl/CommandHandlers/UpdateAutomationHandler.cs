using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Commands;
using Montr.Automate.Services;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Automate.Impl.CommandHandlers
{
	public class UpdateAutomationHandler : IRequestHandler<UpdateAutomation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IAutomationService _automationService;

		public UpdateAutomationHandler(IUnitOfWorkFactory unitOfWorkFactory, IAutomationService automationService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_automationService = automationService;
		}

		public async Task<ApiResult> Handle(UpdateAutomation request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _automationService.Update(request, cancellationToken);

				scope.Commit();

				return result;
			}
		}
	}
}
