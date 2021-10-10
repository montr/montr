using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Tasks.Commands;
using Montr.Tasks.Services;

namespace Montr.Tasks.Impl.CommandHandlers
{
	public class InsertTaskHandler : IRequestHandler<InsertTask, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly ITaskService _taskService;

		public InsertTaskHandler(IUnitOfWorkFactory unitOfWorkFactory, ITaskService taskService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_taskService = taskService;
		}

		public async Task<ApiResult> Handle(InsertTask request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _taskService.Insert(item, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
