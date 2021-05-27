using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Worker.Hangfire.Services
{
	public class ContentProvider : IContentProvider
	{
		public IEnumerable<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.AdminMenu)
			{
				return new[]
				{
					new Menu { Id = "hangfire", Name = "Hangfire Dashboard", Url = "/hangfire/" }
				};
			}

			return null;
		}
	}
}
