using System;
using MediatR;
using Montr.Tendr.Models;

namespace Montr.Tendr.Commands
{
	public class UpdateEvent: IRequest
	{
		public Guid UserUid { get; set; }

		public Event Event { get; set; }
	}
}
