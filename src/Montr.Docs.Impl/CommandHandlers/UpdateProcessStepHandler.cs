using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Services;

namespace Montr.Docs.Impl.CommandHandlers
{
	public class UpdateProcessStepHandler : IRequestHandler<UpdateProcessStep, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IProcessService _processService;

		public UpdateProcessStepHandler(IUnitOfWorkFactory unitOfWorkFactory, IProcessService processService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_processService = processService;
		}

		public async Task<ApiResult> Handle(UpdateProcessStep request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _processService.Update(request, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
