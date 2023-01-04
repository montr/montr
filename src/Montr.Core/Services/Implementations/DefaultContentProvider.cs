using System.Collections.Generic;
using Montr.Core.Models;

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
					new Menu
					{
						Name = "Dashboard",
						Icon = "dashboard",
						Route = ClientRoutes.Dashboard,
						Permission = Permission.GetCode(typeof(Permissions.ViewDashboard))
					},
					new Menu { Name = "Registration", Icon = "user-add", Route = "/registration/" },
				};
			}

			if (menuId == MenuCode.SettingsMenu)
			{
				return new[]
				{
					new Menu { Name = "Common", Route = ClientRoutes.Settings },
					new Menu { Name = "Advanced", Position = 1000 },
					new Menu { Name = "History", Position = 1001 }
				};
			}

			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu { Name = "Registration", Icon = "user-add", Route = "/registration/" },
					new Menu
					{
						Name = "Dashboard",
						Icon = "dashboard",
						Route = ClientRoutes.Dashboard,
						Permission = Permission.GetCode(typeof(Permissions.ViewDashboard))
					},
					new Menu
					{
						Name = "Reports",
						Icon = "bar-chart",
						Route = ClientRoutes.Reports,
						Permission = Permission.GetCode(typeof(Permissions.ViewReports))
					},

					new Menu
					{
						Id = MenuCode.AdminMenu,
						Name = "Administration",
						Icon = "setting",
						Position = 100,
						Items = new[]
						{
							new Menu { Name = "Marketplace", Route = "/marketplace/", Permission = Permission.GetCode(typeof(Permissions.ViewMarketplace)) },
							new Menu { Name = "Integrations", Route = "/integrations/", Permission = Permission.GetCode(typeof(Permissions.ViewIntegrations)) },
							new Menu { Name = "Settings", Route = "/settings/", Permission = Permission.GetCode(typeof(Permissions.ManageSettings)) },
							new Menu { Name = "Localization", Route = "/locales/", Permission = Permission.GetCode(typeof(Permissions.ViewLocales)) }
						}
					},

					// new Menu { Name = "Promo", Icon = "global", Position = 200, Route = "/" }
				};
			}

			return null;
		}
	}
}
