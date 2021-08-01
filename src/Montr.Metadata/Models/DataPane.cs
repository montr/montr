using Montr.Core.Services;

namespace Montr.Metadata.Models
{
	public class ConfigurationItem : IConfigurationItem
	{
		public string Permission { get; set; }

		public int DisplayOrder { get; set; }

		public string Key { get; set; }

		public string Name { get; set; }

		public string Icon { get; set; }

		public string Component { get; set; }

		public object Props { get; set; }
	}

	public class Button : ConfigurationItem
	{
		public string Action { get; set; }
	}

	public class DataPane : ConfigurationItem
	{
	}
}
