using System;
using MediatR;

namespace Montr.Tendr.Commands
{
	public class CancelEvent: IRequest
	{
		public Guid UserUid { get; set; }

		public long EventId { get; set; }
	}
}
