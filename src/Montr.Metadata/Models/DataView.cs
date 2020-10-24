using System.Collections.Generic;

namespace Montr.Metadata.Models
{
	public class DataView
	{
		public string Id { get; set; }

		public IList<DataColumn> Columns { get; set; }

		public IList<DataPane> Panes { get; set; }

		public IList<FieldMetadata> Fields { get; set; }
	}
}
