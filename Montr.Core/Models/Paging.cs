namespace Montr.Core.Models
{
	public class Paging
	{
		public int PageNo { get; set; }

		public int PageSize { get; set; }

		public string SortColumn { get; set; }

		public SortOrder? SortOrder { get; set; }
	}

	public enum SortOrder
	{
		Ascending,

		Descending
	}
}
