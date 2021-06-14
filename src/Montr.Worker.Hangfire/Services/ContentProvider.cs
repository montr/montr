using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Worker.Permissions;

namespace Montr.Worker.Hangfire.Services
{
	public class ContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.AdminMenu)
			{
				return new[]
				{
					new Menu
					{
						Id = "hangfire", Name = "Hangfire Dashboard", Url = "/hangfire/",
						Permission = Permission.GetCode(typeof(ViewDashboard))
					}
				};
			}

			return null;
		}
	}
}
