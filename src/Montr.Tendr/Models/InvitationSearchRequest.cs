using System;
using Montr.Core.Models;

namespace Montr.Tendr.Models
{
	public class InvitationSearchRequest : Paging
	{
		public Guid CompanyUid { get; set; }

		public Guid EventUid { get; set; }
	}
}
