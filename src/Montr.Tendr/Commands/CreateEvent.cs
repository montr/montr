using System;
using MediatR;
using Montr.Tendr.Models;

namespace Montr.Tendr.Commands
{
	public class CreateEvent: IRequest<long>
	{
		public Guid UserUid { get; set; }

		public Event Event { get; set; }
	}
}
