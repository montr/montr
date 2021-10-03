using System;
using MediatR;
using Montr.Core.Models;
using Montr.Tasks.Models;

namespace Montr.Tasks.Commands
{
	public class InsertTask : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public TaskModel Item { get; set; }
	}
}
