using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.Metadata.Models;
using Montr.Web.Services;

namespace Montr.MasterData.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ClassifierLinkController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public ClassifierLinkController(IMediator mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<SearchResult<ClassifierLink>> List(ClassifierLinkSearchRequest request)
		{
			return await _mediator.Send(new GetClassifierLinkList
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Request = request
			});
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertClassifierLink request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteClassifierLink request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
