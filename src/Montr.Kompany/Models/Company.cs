using System.Diagnostics;
using Montr.Metadata.Models;

namespace Montr.Kompany.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Company : IFieldDataContainer
	{
		private string DebuggerDisplay => $"{ConfigCode}, {Name}";

		public static readonly string EntityTypeCode = typeof(Company).Name;

		public System.Guid? Uid { get; set; }

		// public long Id { get; set; }

		public string ConfigCode { get; set; }

		public string StatusCode { get; set; }

		public string Name { get; set; }

		// public string Description { get; set; }

		// public string Url { get; set; }

		public FieldData Fields { get; set; }
	}
}
