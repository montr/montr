using System.Collections.Generic;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Idx.Impl.Services
{
	public class ContentProvider : IContentProvider
	{
		public IList<Menu> GetMenuItems(string menuId)
		{
			if (menuId == MenuCode.ProfileMenu)
			{
				return new[]
				{
					new Menu { Id = "p.0", Name = "Profile", Route = "/profile/" },
					new Menu { Id = "p.1", Name = "Security", Route = "/profile/security/" },
					new Menu { Id = "p.2", Name = "External Logins", Route = "/profile/external-login/" },
					new Menu { Id = "p.3", Name = "Notifications", Route = "/profile/notifications/" },
					new Menu { Id = "p.4", Name = "History", Route = "/profile/history/" }
				};
			}

			if (menuId == MenuCode.SettingsMenu)
			{
				return new[]
				{
					new Menu { Id = "p.1", Name = "SSO Applications", Route = "/settings/sso-applications/" },
					new Menu { Id = "p.2", Name = "SSO Providers", Route = "/settings/sso-providers/" }
				};
			}

			return null;
		}
	}
}
