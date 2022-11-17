using System.Collections.Generic;
using Montr.Metadata.Models;

namespace Montr.Settings.Models
{
	public class SettingsBlock
	{
		public string TypeCode { get; set; }
		
		public string DisplayName { get; set; }
		
		public ICollection<FieldMetadata> Fields { get; set; }
	}
}
