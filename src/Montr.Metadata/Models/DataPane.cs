using Montr.Core.Models;

namespace Montr.Metadata.Models
{
	public class Button : ConfigurationItem
	{
		public ButtonType Type { get; set; }

		public string Action { get; set; }
	}

	public class ButtonEdit : Button
	{
		public ButtonEdit()
		{
			Component = Core.ComponentCode.ButtonEdit;
		}
	}

	public enum ButtonType
	{
		Default,
		Primary
	}

	public class DataPane : ConfigurationItem
	{
	}

	public class DataPanel : ConfigurationItem
	{
	}
}
