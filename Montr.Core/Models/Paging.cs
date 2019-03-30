namespace Montr.Core.Models
{
	public class Paging
	{
		public const int DefaultPageSize = 10;

		public const int MaxPageSize = 10000;

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
