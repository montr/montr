using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Tasks.Services.Implementations
{
	public class ContentProvider: IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu
					{
						Name = "Tasks",
						Icon = "carry-out",
						Route = ClientRoutes.Tasks,
						Permission = Permission.GetCode(typeof(Permissions.ViewTasks))
					}
				};
			}

			return null;
		}
	}
}
