using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Commands;
using Montr.Core.Models;
using Montr.Core.Queries;

namespace Montr.Core.Controllers
{
	[/*Authorize,*/ ApiController, Route("api/[controller]/[action]")]
	public class LocaleController : ControllerBase
	{
		private readonly ISender _mediator;

		public LocaleController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpGet, Route("{locale}/{module}")]
		public async Task<IDictionary<string, string>> Strings([FromRoute]string locale, [FromRoute]string module)
		{
			return await _mediator.Send(new GetAllLocaleStrings { Locale = locale, Module = module });
		}

		[HttpPost]
		public async Task<SearchResult<LocaleString>> List(GetLocaleStringList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<FileStreamResult> Export(ExportLocaleStringList request)
		{
			var result = await _mediator.Send(request);

			return File(result.Stream, result.ContentType, result.FileName);
		}

		[HttpPost]
		public async Task<ApiResult> Import([FromForm]ImportLocaleStringList request)
		{
			return await _mediator.Send(request);
		}
	}
}
