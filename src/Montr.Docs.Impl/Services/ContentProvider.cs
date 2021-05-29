using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Docs.Impl.Services
{
	public class ContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu { Id = "documents", Name = "Документы", Icon = "container", Route = "/documents/" },
				};
			}

			return null;
		}
	}
}
