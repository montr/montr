using System.Collections.Generic;
using System.Threading.Tasks;
using Montr.Web.Models;

namespace Montr.Web.Services
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
				result.Items.Add(new Menu { Id = "p.1", Name = "Регистрация", Icon = "user-add", Route = "/register/" });
			}

			if (menuId == "SideMenu")
			{
				var classifiers = new Menu { Id = "m.class", Name = "Справочники", Icon = "solution" };
				classifiers.Items = new List<Menu>
				{
					new Menu { Id = "c.1", Name = "Контрагенты", Route = "/classifiers/contractor" },
					new Menu { Id = "c.2", Name = "Виды деятельности", Route = "/classifiers/activity" },
					new Menu { Id = "c.3", Name = "Номенклатура", Route = "/classifiers/product" },
					new Menu { Id = "c.4", Name = "Единицы измерения", Route = "/classifiers/unit" },
					new Menu { Id = "c.5", Name = "Регионы", Route = "/classifiers/region" },
					new Menu { Id = "c.6", Name = "ОК видов экономической деятельности (ОКВЭД2)", Route = "/classifiers/okved2" },
					new Menu { Id = "c.7", Name = "ОК единиц измерения", Route = "/classifiers/okei" },
				};

				result.Items.Add(new Menu { Id = "m.0", Name = "Панель управления", Icon = "dashboard", Route = "/" });
				result.Items.Add(new Menu { Id = "m.1", Name = "Процедуры", Icon = "project", Route = "/events" });
				result.Items.Add(classifiers);
				result.Items.Add(new Menu { Id = "m.3", Name = "Настройки", Icon = "setting", Route = "/settings" });

				result.Items.Add(new Menu { Id = "tender", Name = "Промо", Icon = "global", Url = "http://tendr.montr.io:5000/register/" });
			}

			return await Task.FromResult(result);
		}
	}
}
