using Montr.Core.Models;

namespace Montr.Metadata.Models
{
	// todo: rename to TableColumn (?)
	public class DataColumn
	{
		public string Key { get; set; }

		public string Type { get; set; }

		public string Path { get; set; }

		public string Name { get; set; }

		// todo: convert to bool to show link, generate link on client (?)
		public string UrlProperty { get; set; }

		public DataColumnAlign Align { get; set; }

		public DataColumnFixed? Fixed { get; set; }

		public bool Sortable { get; set; }

		public SortOrder? DefaultSortOrder { get; set; }

		public short? Width { get; set; }
	}
}
