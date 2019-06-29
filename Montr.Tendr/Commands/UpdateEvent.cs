using System;
using MediatR;
using Tendr.Models;

namespace Tendr.Commands
{
	public class UpdateEvent: IRequest
	{
		public Guid UserUid { get; set; }

		public Event Event { get; set; }
	}
}
