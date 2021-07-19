using Montr.Core.Services;

namespace Montr.Metadata.Models
{
	public class DataPane : IConfigurationItem
	{
		public string Permission { get; set; }

		public int DisplayOrder { get; set; }

		public string Key { get; set; }

		public string Name { get; set; }

		public string Icon { get; set; }

		public string Component { get; set; }

		public object Props { get; set; }
	}
}
