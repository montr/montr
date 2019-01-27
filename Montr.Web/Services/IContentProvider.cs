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
				result.Items.Add(new Menu { Id = "tender", Name = "Тендеры", Url = "http://tendr.montr.io:5000/register/" });
				result.Items.Add(new Menu { Id = "tender.app", Name = "Тендеры (кабинет)", Url = "http://app.tendr.montr.io:5000/" });
				result.Items.Add(new Menu { Id = "company", Name = "Компания", Url = "http://kompany.montr.io:5010/" });
			}

			if (menuId == "SideMenu")
			{
				result.Items.Add(new Menu { Id = "m.0", Name = "Панель управления", Icon = "dashboard", Route = "/" });
				result.Items.Add(new Menu { Id = "m.1", Name = "Торговые процедуры", Icon = "project", Route = "/events" });
				result.Items.Add(new Menu { Id = "m.2", Name = "Настройки", Icon = "setting" });
				result.Items.Add(new Menu { Id = "tender", Name = "Промо", Icon = "global", Url = "http://tendr.montr.io:5000/register/" });
			}

			return await Task.FromResult(result);
		}
	}
}
