using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Permissions;

namespace Montr.Core.Services.Implementations
{
	public class DefaultContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.TopMenu)
			{
				return new[]
				{
					new Menu { Icon = "home", Route = "/" },
					// new Menu { Id = "login", Name = "Login", Icon = "login", Route = "/account/login/" },
					// new Menu { Id = "register", Name = "Registration", Icon = "user-add", Route = "/account/register/" },
					new Menu { Name = "Registration", Icon = "user-add", Route = "/registration/" },
					// new Menu { Name = "Dashboard", Icon = "dashboard", Route = "/dashboard/" }
				};
			}

			if (menuId == MenuCode.SettingsMenu)
			{
				return new[]
				{
					new Menu { Name = "Common", Route = "/settings/" },
					new Menu { Name = "Advanced", Position = 1000 },
					new Menu { Name = "History", Position = 1001 }
				};
			}

			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu { Name = "Registration", Icon = "user-add", Route = "/registration/" },

					new Menu { Name = "Dashboard", Icon = "dashboard", Route = "/dashboard/" },
					new Menu { Name = "Reports", Icon = "bar-chart", Route = "/reports/" },

					new Menu
					{
						Id = MenuCode.AdminMenu,
						Name = "Administration",
						Icon = "setting",
						Position = 100,
						Items = new[]
						{
							new Menu { Name = "Marketplace", Route = "/marketplace/" },
							new Menu { Name = "Integrations", Route = "/integrations/" },
							new Menu { Name = "Settings", Route = "/settings/", Permission = Permission.GetCode(typeof(ManageSettings)) },
							new Menu { Name = "Localization", Route = "/locales/", Permission = Permission.GetCode(typeof(ViewLocales)) }
						}
					},

					new Menu { Name = "Promo", Icon = "global", Position = 200, Route = "/" }
				};
			}

			return null;
		}
	}
}
