using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Services;
using Montr.Kompany.Services;
using Montr.MasterData.Queries;
using Montr.Metadata.Models;

namespace Montr.MasterData.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ClassifierMetadataController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentCompanyProvider _currentCompanyProvider;
		private readonly ICurrentUserProvider _currentUserProvider;

		public ClassifierMetadataController(IMediator mediator,
			ICurrentCompanyProvider currentCompanyProvider, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentCompanyProvider = currentCompanyProvider;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<DataView> View(GetClassifierMetadata request)
		{
			request.CompanyUid = await _currentCompanyProvider.GetCompanyUid();
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
