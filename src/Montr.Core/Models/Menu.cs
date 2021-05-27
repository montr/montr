using System.Collections.Generic;

namespace Montr.Core.Models
{
	public class Menu
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Icon { get; set; }

		public int? Position { get; set; }

		public string Url { get; set; }

		public string Route { get; set; }

		public IList<Menu> Items { get; set; }
	}

	public class MenuCode
	{
		public static readonly string TopMenu = "TopMenu";

		public static readonly string ProfileMenu = "ProfileMenu";

		public static readonly string SettingsMenu = "SettingsMenu";

		public static readonly string SideMenu = "SideMenu";

		public static readonly string AdminMenu = "AdminMenu";
	}
}
