using Montr.MasterData.Models;

namespace Montr.Automate.Models
{
	public class AutomationSearchRequest : ClassifierSearchRequest
	{
		public string EntityTypeCode { get; set; }

		public bool IncludeRules { get; set; }
	}
}
