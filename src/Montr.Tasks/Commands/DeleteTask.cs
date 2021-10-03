using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Tasks.Commands
{
	public class DeleteTask : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid[] Uids { get; set; }
	}
}
