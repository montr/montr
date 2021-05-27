using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultContentProvider : IContentProvider
	{
		public IEnumerable<Menu> GetMenuItems(string menuId)
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

			if (menuId == MenuCode.ProfileMenu)
			{
				return new[]
				{
					new Menu { Id = "p.0", Name = "Profile", Route = "/profile/" },
					new Menu { Id = "p.1", Name = "Security", Route = "/profile/security/" },
					new Menu { Id = "p.2", Name = "External Logins", Route = "/profile/external-login/" },
					new Menu { Id = "p.3", Name = "Notifications", Route = "/profile/notifications/" },
					new Menu { Id = "p.4", Name = "History", Route = "/profile/history/" }
				};
			}

			if (menuId == MenuCode.SettingsMenu)
			{
				return new[]
				{
					new Menu { Id = "p.0", Name = "SMTP", Route = "/settings/smtp/" },
					new Menu { Id = "p.1", Name = "SSO Applications", Route = "/settings/sso-applications/" },
					new Menu { Id = "p.2", Name = "SSO Providers", Route = "/settings/sso-providers/" }
				};
			}

			if (menuId == MenuCode.SideMenu)
			{
				return new[]
				{
					new Menu { Id = "m.0", Name = "Панель управления", Icon = "dashboard", Route = "/dashboard/" },
					new Menu { Id = "m.1", Name = "Процедуры", Icon = "project", Route = "/events/" },
					new Menu { Id = "reports", Name = "Отчеты", Icon = "bar-chart", Route = "/reports/" },
					new Menu { Id = "documents", Name = "Документы", Icon = "container", Route = "/documents/" },

					new Menu
					{
						Id = MenuCode.AdminMenu,
						Name = "Администрирование",
						Icon = "setting",
						Items = new[]
						{
							new Menu { Id = "companies", Name = "Компании", Route = "/companies/" },
							// new Menu { Id = "integrations", Name = "Интеграции", Route = "/integrations/" },
							new Menu { Id = "classifiers", Name = "Классификаторы", Route = "/classifiers/" },
							new Menu { Id = "settings", Name = "Настройки", Route = "/settings/" },
							new Menu { Id = "locales", Name = "Локализация", Route = "/locales/" }
						}
					},

					new Menu { Id = "promo", Name = "Промо", Icon = "global", Route = "/" }
				};
			}

			return null;
		}
	}
}
