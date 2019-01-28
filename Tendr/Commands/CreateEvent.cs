using System;
using MediatR;
using Tendr.Models;

namespace Tendr.Commands
{
	public class CreateEvent: IRequest<long>
	{
		public Guid UserUid { get; set; }

		public Event Event { get; set; }
	}
}
