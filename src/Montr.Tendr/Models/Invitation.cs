using Montr.MasterData.Models;

namespace Montr.Tendr.Models
{
	public class Invitation
	{
		public System.Guid Uid { get; set; }

		public string StatusCode { get; set; }

		public System.Guid CounterpartyUid  { get; set; }

		public Classifier Counterparty { get; set; }
	}
}
