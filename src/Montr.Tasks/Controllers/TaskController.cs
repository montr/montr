using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Tasks.Commands;
using Montr.Tasks.Models;
using Montr.Tasks.Permissions;
using Montr.Tasks.Queries;

namespace Montr.Tasks.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class TaskController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public TaskController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<DataView> Metadata(GetTaskMetadata request)
		{
			request.Principal = User;

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ViewTasks))]
		public async Task<SearchResult<TaskModel>> List(GetTaskList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ViewTasks))]
		public async Task<TaskModel> Get(GetTask request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageTasks))]
		public async Task<TaskModel> Create(CreateTask request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageTasks))]
		public async Task<ApiResult> Insert(InsertTask request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageTasks))]
		public async Task<ApiResult> Update(UpdateTask request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageTasks))]
		public async Task<ApiResult> Delete(DeleteTask request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
