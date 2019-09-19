using System;

namespace Montr.Tendr.Models
{
	public class Invitation
	{
		public Guid Uid { get; set; }

		public string StatusCode { get; set; }

		public Guid CounterpartyUid  { get; set; }

		public string Email { get; set; }
	}
}
