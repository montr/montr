using System;

namespace Montr.Tendr.Models
{
	public class InvitationListItem
	{
		public Guid Uid { get; set; }

		public string StatusCode { get; set; }

		public string StatusName { get; set; }

		public Guid CounterpartyUid  { get; set; }

		public string CounterpartyName { get; set; }

		public string Email { get; set; }
	}
}
