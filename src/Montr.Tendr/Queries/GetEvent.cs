using System;
using MediatR;
using Montr.Tendr.Models;

namespace Montr.Tendr.Queries
{
	public class GetEvent : IRequest<Event>
	{
		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }

		public Guid Uid { get; set; }
	}
}
