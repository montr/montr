using System;
using System.Collections.Generic;
using System.Linq;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultContentService : IContentService
	{
		private readonly IEnumerable<IContentProvider> _providers;

		private readonly IDictionary<string, Menu> _menus = new Dictionary<string, Menu>(StringComparer.OrdinalIgnoreCase);

		public DefaultContentService(IEnumerable<IContentProvider> providers)
		{
			_providers = providers;
		}

		public void Rebuild()
		{
			var menus = new[] { MenuCode.TopMenu, MenuCode.ProfileMenu, MenuCode.SettingsMenu, MenuCode.SideMenu };

			foreach (var menuId in menus)
			{
				var items = new List<Menu>();

				foreach (var provider in _providers)
				{
					var children = provider.GetMenuItems(menuId);

					if (children != null) items.AddRange(children);
				}

				_menus[menuId] = new Menu
				{
					Id = menuId,
					Items = items.OrderBy(x => x.Position).ToList()
				};
			}
		}

		public Menu GetMenu(string menuId)
		{
			return _menus.TryGetValue(menuId, out var menu) ? menu : null;
		}
	}
}
