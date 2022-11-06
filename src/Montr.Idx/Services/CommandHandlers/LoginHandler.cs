using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Entities;

namespace Montr.Idx.Services.CommandHandlers
{
	public class LoginHandler : IRequestHandler<Login, ApiResult>
	{
		private readonly ILogger<ConfirmEmailHandler> _logger;
		private readonly ILocalizer _localizer;
		private readonly SignInManager<DbUser> _signInManager;

		public LoginHandler(
			ILogger<ConfirmEmailHandler> logger,
			ILocalizer localizer,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_localizer = localizer;
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
				return new ApiResult { Success = false, Message = await _localizer.Get<Login.Resources>(x => x.RequiresTwoFactor, cancellationToken) };
			}

			if (result.IsLockedOut)
			{
				_logger.LogWarning("User account locked out.");

				return new ApiResult { Success = false, Message = await _localizer.Get<Login.Resources>(x => x.IsLockedOut, cancellationToken) };
			}

			if (result.IsNotAllowed)
			{
				return new ApiResult { Success = false, Message = await _localizer.Get<Login.Resources>(x => x.IsNotAllowed, cancellationToken) };
			}

			return new ApiResult { Success = false, Message = await _localizer.Get<Login.Resources>(x => x.Error, cancellationToken) };
		}
	}
}
