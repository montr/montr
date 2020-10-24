using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Idx.Services;
using Montr.Metadata.Commands;
using Montr.Metadata.Models;
using Montr.Metadata.Queries;

namespace Montr.Metadata.Controllers
{
	[/* Authorize, */ ApiController, Route("api/[controller]/[action]")]
	public class MetadataController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public MetadataController(IMediator mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
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
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<FieldMetadata> Get(GetDataField request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertDataField request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateDataField request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteDataField request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
