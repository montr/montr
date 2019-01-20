using System;
using System.Threading.Tasks;
using Kompany.Commands;
using Kompany.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Web.Services;

namespace Kompany.Web.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class CompanyController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public CompanyController(IMediator mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[Authorize, HttpPost]
		public async Task<ActionResult<Guid>> Create(Company item)
		{
			return await _mediator.Send(new CreateCompany
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Company = item
			});
		}
	}
}
