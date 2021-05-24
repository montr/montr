using System.Collections.Generic;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IContentProvider
	{
		/// <summary>
		/// Get children items for specified menu item.
		/// </summary>
		/// <param name="menuId"></param>
		IEnumerable<Menu> GetMenuItems(string menuId);
	}
}
