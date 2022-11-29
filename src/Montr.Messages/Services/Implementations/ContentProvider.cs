using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Messages.Services.Implementations
{
	public class ContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.SettingsMenu)
			{
				return new[]
				{
					new Menu { Name = "SMTP", Route = "/settings/" + SettingsCategory.Smtp }
				};
			}

			return null;
		}
	}
}
