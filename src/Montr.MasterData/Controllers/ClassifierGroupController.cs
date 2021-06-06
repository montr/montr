using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Permissions;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ClassifierGroupController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public ClassifierGroupController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost, Permission(typeof(ViewClassifiers))]
		public async Task<SearchResult<ClassifierGroup>> List(GetClassifierGroupList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ViewClassifiers))]
		public async Task<ClassifierGroup> Get(GetClassifierGroup request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifiers))]
		public async Task<ApiResult> Insert(InsertClassifierGroup request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifiers))]
		public async Task<ApiResult> Update(UpdateClassifierGroup request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifiers))]
		public async Task<ApiResult> Delete(DeleteClassifierGroup request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
