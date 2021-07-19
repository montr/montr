using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	/// <summary>
	/// Used to collect content items (menus etc.) from registered content providers.
	/// </summary>
	public class DefaultContentService : IContentService
	{
		private readonly IEnumerable<IContentProvider> _providers;

		private readonly ConcurrentDictionary<string, Menu> _menus = new(StringComparer.OrdinalIgnoreCase);

		public DefaultContentService(IEnumerable<IContentProvider> providers)
		{
			_providers = providers;
		}

		// todo: refactor, check cyclic menus
		private IList<Menu> Rebuild(string menuId)
		{
			var result = new List<Menu>();

			foreach (var provider in _providers)
			{
				var items = provider.GetMenuItems(menuId);

				if (items != null)
				{
					foreach (var item in items)
					{
						result.Add(item);

						if (item.Id != null)
						{
							var children = Rebuild(item.Id);

							if (children?.Count > 0)
							{
								if (item.Items == null)
								{
									item.Items = new List<Menu>();
								}
								else if (item.Items.IsReadOnly)
								{
									item.Items = item.Items.ToList();
								}

								foreach (var child in children)
								{
									item.Items.Add(child);
								}

								item.Items = item.Items.OrderBy(x => x.Position).ToList();
							}
						}
					}
				}
			}

			return result.OrderBy(x => x.Position).ToList();
		}

		public Menu GetMenu(string menuId)
		{
			return _menus.GetOrAdd(menuId, x => new Menu { Id = x, Items = Rebuild(x) });
		}
	}
}
