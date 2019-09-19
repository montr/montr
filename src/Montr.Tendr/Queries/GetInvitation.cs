using System;
using MediatR;
using Montr.Tendr.Models;

namespace Montr.Tendr.Queries
{
	public class GetInvitation : IRequest<Invitation>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public Guid Uid { get; set; }
	}
}
