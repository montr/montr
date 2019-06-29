using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kompany.Commands;
using Kompany.Models;
using Kompany.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Web.Services;

namespace Kompany.Controllers
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

		[HttpPost]
		public async Task<ActionResult<IList<Company>>> List()
		{
			var result = await _mediator.Send(new GetCompanyList
			{
				UserUid = _currentUserProvider.GetUserUid()
			});

			return Ok(result);
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
