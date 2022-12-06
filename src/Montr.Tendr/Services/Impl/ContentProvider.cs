using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Tendr.Services.Impl
{
	public class ContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu
					{
						Name = "Events",
						Icon = "project",
						Route = ClientRoutes.Events,
						Permission = Permission.GetCode(typeof(Permissions.ViewEvents))
					}
				};
			}

			return null;
		}
	}
}
