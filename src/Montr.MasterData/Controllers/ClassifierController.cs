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
	public class ClassifierController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public ClassifierController(IMediator mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost]
		public async Task<SearchResult<Classifier>> List(ClassifierSearchRequest request)
		{
			return await _mediator.Send(new GetClassifierList
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Request = request
			});
		}

		[HttpPost]
		public async Task<FileStreamResult> Export(ClassifierSearchRequest request)
		{
			var result = await _mediator.Send(new ExportClassifierList
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Request = request
			});

			// http://blog.stephencleary.com/2016/11/streaming-zip-on-aspnet-core.html

			return File(result.Stream, result.ContentType, result.FileName);
		}

		[HttpPost]
		public async Task<Classifier> Get(GetClassifier request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Insert(InsertClassifier request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Update(UpdateClassifier request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost]
		public async Task<ApiResult> Delete(DeleteClassifierList request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
