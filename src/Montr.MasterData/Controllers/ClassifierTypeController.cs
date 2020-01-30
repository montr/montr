using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Services;
using Montr.Kompany.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ClassifierTypeController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentCompanyProvider _currentCompanyProvider;
		private readonly ICurrentUserProvider _currentUserProvider;

		public ClassifierTypeController(IMediator mediator,
			ICurrentCompanyProvider currentCompanyProvider, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentCompanyProvider = currentCompanyProvider;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<SearchResult<ClassifierType>> List(GetClassifierTypeList request)
		{
			// request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ClassifierType> Get(GetClassifierType request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertClassifierType request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateClassifierType request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteClassifierType request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
