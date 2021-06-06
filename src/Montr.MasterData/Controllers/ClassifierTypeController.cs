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
	public class ClassifierTypeController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public ClassifierTypeController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost, Permission(typeof(ViewClassifierTypes))]
		public async Task<SearchResult<ClassifierType>> List(GetClassifierTypeList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ViewClassifierTypes))]
		public async Task<ClassifierType> Get(GetClassifierType request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifierTypes))]
		public async Task<ApiResult> Insert(InsertClassifierType request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifierTypes))]
		public async Task<ApiResult> Update(UpdateClassifierType request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifierTypes))]
		public async Task<ApiResult> Delete(DeleteClassifierType request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
