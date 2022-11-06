using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Tasks.Commands;

namespace Montr.Tasks.Services.CommandHandlers
{
	public class UpdateTaskHandler : IRequestHandler<UpdateTask, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly ITaskService _taskService;

		public UpdateTaskHandler(IUnitOfWorkFactory unitOfWorkFactory, ITaskService taskService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_taskService = taskService;
		}

		public async Task<ApiResult> Handle(UpdateTask request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _taskService.Update(item, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
