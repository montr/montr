using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Metadata.Commands;
using Montr.Metadata.Models;
using Montr.Metadata.Queries;

namespace Montr.Metadata.Controllers
{
	[/* Authorize, */ ApiController, Route("api/[controller]/[action]")]
	public class MetadataController : ControllerBase
	{
		private readonly ISender _mediator;

		public MetadataController(ISender mediator)
		{
			_mediator = mediator;
		}

		[HttpPost]
		public async Task<DataView> View(GetMetadata request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<SearchResult<FieldMetadata>> List(GetMetadataList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<IList<FieldType>> FieldTypes(GetFieldTypes request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<FieldMetadata> Get(GetDataField request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertDataField request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateDataField request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteDataField request)
		{
			return await _mediator.Send(request);
		}
	}
}
