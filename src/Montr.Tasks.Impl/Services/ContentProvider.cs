using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Tasks.Impl.Services
{
	public class ContentProvider: IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu { Id = "tasks", Name = "Tasks", Icon = "carry-out", Route = "/tasks/" },
				};
			}

			return null;
		}
	}
}
