using Montr.MasterData.Models;

namespace Montr.Kompany.Models
{
	public class CompanySearchRequest : ClassifierSearchRequest
	{
		public string ConfigCode { get; set; }
	}
}
