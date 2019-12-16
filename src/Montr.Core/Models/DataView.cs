using System.Collections.Generic;

namespace Montr.Core.Models
{
	public class DataView
	{
		public string Id { get; set; }

		public ICollection<DataColumn> Columns { get; set; }

		public ICollection<DataPane> Panes { get; set; }

		public ICollection<DataField> Fields { get; set; }
	}
}
