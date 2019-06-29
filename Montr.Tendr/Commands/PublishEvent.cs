using System;
using MediatR;

namespace Tendr.Commands
{
	public class PublishEvent: IRequest
	{
		public Guid UserUid { get; set; }

		public long EventId { get; set; }
	}
}