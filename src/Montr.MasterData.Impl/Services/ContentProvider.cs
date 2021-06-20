using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Queries;

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
					new Menu
					{
						Id = "classifiers", Name = "Классификаторы", Route = "/classifiers/",
						Permission = Permission.GetCode(typeof(GetClassifierTypeList))
					}
				};
			}

			return null;
		}
	}
}
