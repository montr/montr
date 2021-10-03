using System;
using MediatR;
using Montr.Tasks.Models;

namespace Montr.Tasks.Queries
{
	public class GetTask : IRequest<TaskModel>
	{
		public Guid Uid { get; set; }
	}
}
