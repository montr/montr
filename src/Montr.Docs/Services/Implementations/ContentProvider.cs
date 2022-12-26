using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Docs.Services.Implementations
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
						Id = "documents",
						Name = "Documents",
						Icon = "container",
						Route = ClientRoutes.Documents,
						Permission = Permission.GetCode(typeof(Permissions.ViewDocuments))
					}
				};
			}

			return null;
		}
	}
}
