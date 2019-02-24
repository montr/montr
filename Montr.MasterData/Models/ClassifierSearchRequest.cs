using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierSearchRequest : SearchRequest
	{
		public string ConfigCode { get; set; }

		public System.Guid CompanyUid { get; set; }
	}
}
