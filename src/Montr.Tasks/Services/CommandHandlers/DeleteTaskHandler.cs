using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Tasks.Commands;

namespace Montr.Tasks.Services.CommandHandlers
{
	public class DeleteTaskHandler : IRequestHandler<DeleteTask, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly ITaskService _taskService;

		public DeleteTaskHandler(IUnitOfWorkFactory unitOfWorkFactory, ITaskService taskService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_taskService = taskService;
		}

		public async Task<ApiResult> Handle(DeleteTask request, CancellationToken cancellationToken)
		{
			var uids = request.Uids ?? throw new ArgumentNullException(nameof(request.Uids));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _taskService.Delete(uids, cancellationToken);
				
				if (result.Success) scope.Commit();
				
				return result;
			}
		}
	}
}
