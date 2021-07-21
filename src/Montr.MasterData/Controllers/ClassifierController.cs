using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Permissions;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Controllers
{
	[Authorize, ApiController, Route("api/[controller]/[action]")]
	public class ClassifierController : ControllerBase
	{
		private readonly ISender _mediator;
		private readonly ICurrentUserProvider _currentUserProvider;

		public ClassifierController(ISender mediator, ICurrentUserProvider currentUserProvider)
		{
			_mediator = mediator;
			_currentUserProvider = currentUserProvider;
		}

		[HttpPost, Permission(typeof(ViewClassifiers))]
		public async Task<SearchResult<Classifier>> List(GetClassifierList request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ViewClassifiers))]
		public async Task<FileStreamResult> Export(ExportClassifierList request)
		{
			var result = await _mediator.Send(request);

			// http://blog.stephencleary.com/2016/11/streaming-zip-on-aspnet-core.html

			return File(result.Stream, result.ContentType, result.FileName);
		}

		[HttpPost, Permission(typeof(ManageClassifiers))]
		public async Task<Classifier> Create(CreateClassifier request)
		{
			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifiers))]
		public async Task<Classifier> Get(GetClassifier request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifiers))]
		public async Task<ApiResult> Insert(InsertClassifier request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifiers))]
		public async Task<ApiResult> Update(UpdateClassifier request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}

		[HttpPost, Permission(typeof(ManageClassifiers))]
		public async Task<ApiResult> Delete(DeleteClassifier request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
