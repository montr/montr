using System.Collections.Generic;

namespace Montr.Web.Models
{
	public class Menu
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }

		public string Route { get; set; }

		public IList<Menu> Items { get; set; }
	}
}
