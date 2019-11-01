using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, ChallengeResult>,
		IRequestHandler<ExternalLoginCallbackCommand, IActionResult>
	{
		private readonly ILogger<ExternalLoginCommandHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly IAppUrlBuilder _appUrlBuilder;

		public ExternalLoginCommandHandler(
			ILogger<ExternalLoginCommandHandler> logger,
			SignInManager<DbUser> signInManager,
			IAppUrlBuilder appUrlBuilder)
		{
			_logger = logger;
			_signInManager = signInManager;
			_appUrlBuilder = appUrlBuilder;
		}

		public Task<ChallengeResult> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
		{
			// var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });

			var redirectUrl = _appUrlBuilder.Build("/api" + ClientRoutes.ExternalLoginCallback,
				new Dictionary<string, string> { { "returnUrl", request.ReturnUrl } });

			var properties = _signInManager.ConfigureExternalAuthenticationProperties(request.Provider, redirectUrl);
			var result = new ChallengeResult(request.Provider, properties);

			return Task.FromResult(result);
		}

		public async Task<IActionResult> Handle(ExternalLoginCallbackCommand request, CancellationToken cancellationToken)
		{
			var returnUrl = request.ReturnUrl ?? "~/";

			if (request.RemoteError != null)
			{
				// ErrorMessage = $"Error from external provider: {remoteError}";
				return new RedirectResult("./Login"/*, new { ReturnUrl = returnUrl }*/);
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				// ErrorMessage = "Error loading external login information.";
				return new RedirectResult("./Login"/*, new { ReturnUrl = returnUrl }*/);
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
			if (result.Succeeded)
			{
				_logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
				return new RedirectResult(returnUrl);
			}
			if (result.IsLockedOut)
			{
				return new RedirectResult("./Lockout");
			}
			else
			{
				// If the user does not have an account, then ask the user to create an account.
				// ReturnUrl = returnUrl;
				// LoginProvider = info.LoginProvider;
				if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
				{
					var email = info.Principal.FindFirstValue(ClaimTypes.Email);

					/*Input = new InputModel
					{
						Email = info.Principal.FindFirstValue(ClaimTypes.Email)
					};*/
				}

				// return Page();
				return new RedirectResult("./RegisterPage");
			}
		}

	}
}
