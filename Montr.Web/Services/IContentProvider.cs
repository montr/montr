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
				result.Items.Add(new Menu { Id = "company", Name = "Компания", Url = "http://kompany.montr.io:5010/" });
				result.Items.Add(new Menu { Id = "tender", Name = "Тендеры (промо)", Url = "http://tendr.montr.io:5000/" });
				result.Items.Add(new Menu { Id = "tender.app", Name = "Тендеры (кабинет)", Url = "http://app.tendr.montr.io:5000/" });
				result.Items.Add(new Menu { Id = "personal", Name = "Личный кабинет", Url = "http://idx.montr.io:5050/" });
			}

			return await Task.FromResult(result);
		}
	}
}
