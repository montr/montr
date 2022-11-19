using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Idx.Services.Implementations
{
	public class ContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.ProfileMenu)
			{
				return new[]
				{
					new Menu { Name = "Profile", Route = "/profile/" },
					new Menu { Name = "Security", Route = "/profile/security/" },
					new Menu { Name = "External Logins", Route = "/profile/external-login/" },
					new Menu { Name = "Notifications", Route = "/profile/notifications/" },
					new Menu { Name = "History", Route = "/profile/history/" }
				};
			}

			if (menuId == MenuCode.SettingsMenu)
			{
				return new[]
				{
					new Menu { Name = "SSO Applications", Route = "/settings/sso-applications" },
					new Menu { Name = "SSO Providers", Route = "/settings/sso-providers" }
				};
			}

			return null;
		}
	}
}
