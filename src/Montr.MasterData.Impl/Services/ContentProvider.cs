using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.MasterData.Impl.Services
{
	public class ContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.AdminMenu)
			{
				return new[]
				{
					new Menu { Id = "companies", Name = "Компании", Route = "/companies/" },
					new Menu { Id = "classifiers", Name = "Классификаторы", Route = "/classifiers/" }
				};
			}

			return null;
		}
	}
}
