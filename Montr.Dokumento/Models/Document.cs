namespace Dokumento.Models
{
	public class Document
	{
		public System.Guid Uid { get; set; }

		public System.Guid CompanyUid { get; set; }

		public string ConfigCode { get; set; }

		public string StatusCode { get; set; }
	}
}
