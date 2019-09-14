using System;
using MediatR;
using Montr.Tendr.Models;

namespace Montr.Tendr.Queries
{
	public class GetEvent : IRequest<Event>
	{
		public Guid Uid { get; set; }
	}
}
