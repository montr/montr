using System.Collections.Generic;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IContentProvider
	{
		Task<Menu> GetMenu(string menuId);
	}

	public class DefaultContentProvider : IContentProvider
	{
		public async Task<Menu> GetMenu(string menuId)
		{
			var result = new Menu
			{
				Id = menuId,
				Items = new List<Menu>()
			};

			if (menuId == "TopMenu")
			{
				result.Items.Add(new Menu { Id = "p.0", Icon = "home", Route = "/" });
				// result.Items.Add(new Menu { Id = "login", Name = "Войти", Icon = "login", Route = "/account/login" });
				// result.Items.Add(new Menu { Id = "register", Name = "Регистрация", Icon = "user-add", Route = "/account/register" });
				result.Items.Add(new Menu { Id = "p.1", Name = "Регистрация", Icon = "user-add", Route = "/register" });
				// result.Items.Add(new Menu { Id = "p.2", Name = "Панель управления", Icon = "dashboard", Route = "/dashboard" });
			}

			if (menuId == "ProfileMenu")
			{
				result.Items.Add(new Menu { Id = "p.0", Name = "Profile", Route = "/profile/" });
				result.Items.Add(new Menu { Id = "p.1", Name = "Security", Route = "/profile/security/" });
				result.Items.Add(new Menu { Id = "p.2", Name = "External Logins", Route = "/profile/external-login/" });
				result.Items.Add(new Menu { Id = "p.3", Name = "Notifications", Route = "/profile/notifications/" });
				result.Items.Add(new Menu { Id = "p.4", Name = "History", Route = "/profile/history/" });
			}

			if (menuId == "SideMenu")
			{
				result.Items.Add(new Menu { Id = "m.0", Name = "Панель управления", Icon = "dashboard", Route = "/dashboard" });
				result.Items.Add(new Menu { Id = "m.1", Name = "Процедуры", Icon = "project", Route = "/events" });
				result.Items.Add(new Menu { Id = "reports", Name = "Отчеты", Icon = "bar-chart", Route = "/reports" });
				result.Items.Add(new Menu { Id = "documents", Name = "Документы", Icon = "container", Route = "/documents" });

				result.Items.Add(new Menu
				{
					Id = "m.3",
					Name = "Администрирование",
					Icon = "setting",
					Items = new []
					{
						new Menu { Id = "companies", Name = "Компании", Route = "/companies" },
						new Menu { Id = "users", Name = "Пользователи", Route = "/users" },
						new Menu { Id = "roles", Name = "Роли", Route = "/roles" },
						// new Menu { Id = "integrations", Name = "Интеграции", Route = "/integrations" },
						new Menu { Id = "classifiers", Name = "Классификаторы", Route = "/classifiers" },
						new Menu { Id = "settings", Name = "Настройки", Route = "/settings" },
						new Menu { Id = "locales", Name = "Локализация", Route = "/locales" },
						new Menu { Id = "hangfire", Name = "Hangfire Dashboard", Url = "/hangfire" }
					}
				});

				result.Items.Add(new Menu
				{
					Id = "promo",
					Name = "Промо",
					Icon = "global",
					Route = "/"
				});
			}

			return await Task.FromResult(result);
		}
	}
}
