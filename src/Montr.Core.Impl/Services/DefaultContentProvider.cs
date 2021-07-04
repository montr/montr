using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Permissions;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.TopMenu)
			{
				return new[]
				{
					new Menu { Id = "p.0", Icon = "home", Route = "/" },
					// new Menu { Id = "login", Name = "Войти", Icon = "login", Route = "/account/login/" },
					// new Menu { Id = "register", Name = "Регистрация", Icon = "user-add", Route = "/account/register/" },
					new Menu { Id = "p.1", Name = "Регистрация", Icon = "user-add", Route = "/register/" },
					// new Menu { Id = "p.2", Name = "Панель управления", Icon = "dashboard", Route = "/dashboard/" }
				};
			}

			if (menuId == MenuCode.SettingsMenu)
			{
				return new[]
				{
					new Menu { Id = "p.0", Name = "SMTP", Route = "/settings/smtp/" },
				};
			}

			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu { Name = "Панель управления", Icon = "dashboard", Route = "/dashboard/" },
					new Menu { Name = "Отчеты", Icon = "bar-chart", Route = "/reports/" },

					new Menu
					{
						Id = MenuCode.AdminMenu,
						Name = "Администрирование",
						Icon = "setting",
						Position = 100,
						Items = new[]
						{
							// new Menu { Id = "integrations", Name = "Интеграции", Route = "/integrations/" },
							new Menu { Name = "Настройки", Route = "/settings/", Permission = Permission.GetCode(typeof(ManageSettings)) },
							new Menu { Name = "Локализация", Route = "/locales/", Permission = Permission.GetCode(typeof(ViewLocales)) }
						}
					},

					new Menu { Name = "Промо", Icon = "global", Position = 200, Route = "/" }
				};
			}

			return null;
		}
	}
}
