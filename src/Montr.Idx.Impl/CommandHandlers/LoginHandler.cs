using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class LoginHandler : IRequestHandler<Login, ApiResult>
	{
		private readonly ILogger<ConfirmEmailHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;

		public LoginHandler(
			ILogger<ConfirmEmailHandler> logger,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_signInManager = signInManager;
		}

		public async Task<ApiResult> Handle(Login request, CancellationToken cancellationToken)
		{
			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true
			var result = await _signInManager.PasswordSignInAsync(
				request.Email, request.Password, request.RememberMe, lockoutOnFailure: true);

			if (result.Succeeded)
			{
				_logger.LogInformation("User logged in.");

				// todo: check redirect is local
				return new ApiResult { RedirectUrl = request.ReturnUrl };
			}

			if (result.RequiresTwoFactor)
			{
				// return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
				return new ApiResult { RedirectRoute = "./LoginWith2fa", RedirectUrl = request.ReturnUrl };
			}

			if (result.IsLockedOut)
			{
				_logger.LogWarning("User account locked out.");

				// return RedirectToPage("./Lockout");
				return new ApiResult { RedirectRoute = "./Lockout" };
			}

			// todo: implement
			if (result.IsNotAllowed) // Email not confirmed?
			{
			}

			return new ApiResult("Invalid login attempt.");
		}
	}
}
