using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Tendr.Impl.Services
{
	public class ContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu { Id = "m.1", Name = "Events", Icon = "project", Route = "/events/" },
				};
			}

			return null;
		}
	}
}
