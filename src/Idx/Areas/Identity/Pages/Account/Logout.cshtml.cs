using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Montr.Idx.Impl.Entities;

namespace Idx.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class LogoutModel : PageModel
	{
		private readonly SignInManager<DbUser> _signInManager;
		private readonly ILogger<LogoutModel> _logger;

		public LogoutModel(SignInManager<DbUser> signInManager, ILogger<LogoutModel> logger)
		{
			_signInManager = signInManager;
			_logger = logger;
		}

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPost(string returnUrl = null)
		{
			await _signInManager.SignOutAsync();

			_logger.LogInformation("User logged out.");
			if (returnUrl != null)
			{
				return LocalRedirect(returnUrl);
			}
			else
			{
				return Page();
			}
		}
	}
}
