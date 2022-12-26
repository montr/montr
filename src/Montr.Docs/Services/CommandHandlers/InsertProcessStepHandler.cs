using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;

namespace Montr.Docs.Services.CommandHandlers
{
	public class InsertProcessStepHandler : IRequestHandler<InsertProcessStep, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IProcessService _processService;

		public InsertProcessStepHandler(IUnitOfWorkFactory unitOfWorkFactory, IProcessService processService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_processService = processService;
		}

		public async Task<ApiResult> Handle(InsertProcessStep request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _processService.Insert(request, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}