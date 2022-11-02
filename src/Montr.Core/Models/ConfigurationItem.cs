namespace Montr.Core.Models
{
	public interface IConfigurationItem
	{
		string Permission { get; }

		int DisplayOrder { get; }
	}

	public class ConfigurationItem : IConfigurationItem
	{
		public string Permission { get; set; }

		public int DisplayOrder { get; set; }

		public string Key { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Icon { get; set; }

		public string Component { get; set; }

		public object Props { get; set; }
	}
}
