using Montr.Core.Models;

namespace Montr.Metadata.Models
{
	public class Button : ConfigurationItem
	{
		public ButtonType Type { get; set; }

		public string Action { get; set; }
	}

	public enum ButtonType
	{
		Default,
		Primary
	}

	public class DataPane : ConfigurationItem
	{
	}
}
