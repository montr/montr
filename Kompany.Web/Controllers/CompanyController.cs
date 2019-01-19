using System;
using System.Threading.Tasks;
using Kompany.Requests;
using Kompany.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kompany.Web.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class CompanyController : ControllerBase
	{
		private readonly IMediator _mediator;

		public CompanyController(IMediator mediator)
		{
			_mediator = mediator;
		}

		[Authorize, HttpPost]
		public async Task<ActionResult<Guid>> Create(Company item)
		{
			return await _mediator.Send(new CreateCompany { Company = item });
		}
	}
}
