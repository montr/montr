using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Tendr.Models;

namespace Tendr.Controllers
{
	[ApiController, Route("api/[controller]/[action]")]
	public class AccountController : ControllerBase
	{
		[HttpPost]
		public async Task<ActionResult<AccountInfo>> Info()
		{
			var result = new AccountInfo();

			if (User.Identity.IsAuthenticated)
			{
				result.Claims = new Dictionary<string, string>();

				foreach (var claim in User.Claims)
				{
					result.Claims[claim.Type] = claim.Value;
				}

				result.Claims["access_token"] = await HttpContext.GetTokenAsync("access_token");
				result.Claims["refresh_token"] = await HttpContext.GetTokenAsync("refresh_token");
			}

			return result;
		}

		[HttpPost]
		// [ValidateAntiForgeryToken]
		public async Task<ActionResult<ApiResult>> Logout(string returnUrl = null)
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

			return new ApiResult { Success = true };
		}
	}
}