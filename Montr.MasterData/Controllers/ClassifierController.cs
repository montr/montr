﻿using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
		public async Task<ActionResult<DataResult<Classifier>>> List(ClassifierSearchRequest request)
		{
			return await _mediator.Send(new GetClassifierList
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Request = request
			});
		}

		[HttpPost]
		public async Task<ActionResult<Classifier>> Get(Classifier item) // todo: pass only id
		{
			return await _mediator.Send(new GetClassifier
			{
				UserUid = _currentUserProvider.GetUserUid(),
				EntityUid = item.Uid
			});
		}

		[HttpPost]
		public async Task<ActionResult<Guid>> Insert(Classifier item)
		{
			return await _mediator.Send(new InsertClassifier
			{
				UserUid = _currentUserProvider.GetUserUid(),
				CompanyUid = item.CompanyUid,
				Item = item
			});
		}

		[HttpPost]
		public async Task<ActionResult> Update(Classifier item)
		{
			await _mediator.Send(new UpdateClassifier
			{
				UserUid = _currentUserProvider.GetUserUid(),
				Item = item
			});

			return Ok();
		}

		[HttpPost]
		public async Task<int> Delete(DeleteClassifierList request)
		{
			request.UserUid = _currentUserProvider.GetUserUid();

			return await _mediator.Send(request);
		}
	}
}
