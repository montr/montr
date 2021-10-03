using System;
using MediatR;
using Montr.Core.Models;
using Montr.Tasks.Models;

namespace Montr.Tasks.Commands
{
	public class UpdateTask : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public TaskModel Item { get; set; }
	}
}
