using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierSearchRequest : Paging
	{
		public string ConfigCode { get; set; }

		public System.Guid CompanyUid { get; set; }
	}
}
