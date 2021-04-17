﻿using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class DocumentController : ControllerBase
	{
		private readonly ISender _mediator;

		public DocumentController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<SearchResult<Document>> List(GetDocumentList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<Document> Get(GetDocument request)
		{
			return await _mediator.Send(request);
		}
	}
}
