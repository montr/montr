﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Tasks.Models;
using Montr.Tasks.Queries;

namespace Montr.Tasks.Services.QueryHandlers
{
	public class GetTaskHandler : IRequestHandler<GetTask, TaskModel>
	{
		private readonly IRepository<TaskModel> _repository;

		public GetTaskHandler(IRepository<TaskModel> repository)
		{
			_repository = repository;
		}

		public async Task<TaskModel> Handle(GetTask command, CancellationToken cancellationToken)
		{
			var request = new TaskSearchRequest { Uid = command.Uid };

			var result = await _repository.Search(request, cancellationToken);

			return result?.Rows?.SingleOrDefault();
		}
	}
}
