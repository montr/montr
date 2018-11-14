using System.Collections.Generic;

namespace Tendr.Models
{
	public class DataView
	{
		public string Id { get; set; }

		public ICollection<DataColumn> Columns { get; set; }
	}
}
