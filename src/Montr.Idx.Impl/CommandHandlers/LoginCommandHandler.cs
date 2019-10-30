using System.Linq;
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
	public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResult>
	{
		private readonly ILogger<ConfirmEmailCommandHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;

		public LoginCommandHandler(
			ILogger<ConfirmEmailCommandHandler> logger,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_signInManager = signInManager;
		}

		public async Task<ApiResult> Handle(LoginCommand request, CancellationToken cancellationToken)
		{
			var returnUrl = request.ReturnUrl ?? "~/";

			// This doesn't count login failures towards account lockout
			// To enable password failures to trigger account lockout, set lockoutOnFailure: true
			var result = await _signInManager.PasswordSignInAsync(
				request.Email, request.Password, request.RememberMe, lockoutOnFailure: true);

			if (result.Succeeded)
			{
				_logger.LogInformation("User logged in.");

				return new ApiResult { RedirectUrl = returnUrl };
			}

			if (result.RequiresTwoFactor)
			{
				// return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
				return new ApiResult { RedirectRoute = "./LoginWith2fa", RedirectUrl = returnUrl };
			}

			if (result.IsLockedOut)
			{
				_logger.LogWarning("User account locked out.");

				// return RedirectToPage("./Lockout");
				return new ApiResult { RedirectRoute = "./Lockout" };
			}

			if (result.IsNotAllowed) // Email not confirmed?
			{
				// todo
			}

			// ModelState.AddModelError(string.Empty, "Invalid login attempt.");
			return new ApiResult
			{
				Success = false, Errors = new[]
				{
					new ApiResultError { Messages = new[] { "Invalid login attempt." } }
				}
			};
		}
	}
}
