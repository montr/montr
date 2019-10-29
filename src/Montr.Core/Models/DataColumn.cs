namespace Montr.Core.Models
{
	// todo: rename to TableColumn (?)
	public class DataColumn
	{
		public string Key { get; set; }

		public string Path { get; set; }

		public string Name { get; set; }

		public string UrlProperty { get; set; }

		public DataColumnAlign Align { get; set; }

		public DataColumnFixed? Fixed { get; set; }

		public bool Sortable { get; set; }

		public SortOrder? DefaultSortOrder { get; set; }

		public short? Width { get; set; }
	}
}
