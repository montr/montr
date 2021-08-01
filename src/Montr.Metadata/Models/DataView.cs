using System.Collections.Generic;

namespace Montr.Metadata.Models
{
	public class DataView
	{
		public string Id { get; set; }

		public ICollection<Button> Toolbar { get; set; }

		public ICollection<DataPane> Panes { get; set; }

		public ICollection<DataColumn> Columns { get; set; }

		public ICollection<FieldMetadata> Fields { get; set; }
	}
}
