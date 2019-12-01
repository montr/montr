using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ExternalLoginCallbackHandler : IRequestHandler<ExternalLoginCallback, ApiResult<ExternalRegisterModel>>
	{
		private readonly ILogger<ExternalLoginCallbackHandler> _logger;
		private readonly ILocalizer _localizer;
		private readonly SignInManager<DbUser> _signInManager;

		public ExternalLoginCallbackHandler(
			ILogger<ExternalLoginCallbackHandler> logger,
			ILocalizer localizer,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_localizer = localizer;
			_signInManager = signInManager;
		}

		public async Task<ApiResult<ExternalRegisterModel>> Handle(ExternalLoginCallback request, CancellationToken cancellationToken)
		{
			if (request.RemoteError != null)
			{
				return new ApiResult<ExternalRegisterModel> { Success = false, Message = $"Error from external provider: {request.RemoteError}" };
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return new ApiResult<ExternalRegisterModel> { Success = false, Message = "Error loading external login information." };
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

			if (result.Succeeded)
			{
				_logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);

				// todo: check redirect is local
				return new ApiResult<ExternalRegisterModel> { RedirectUrl = request.ReturnUrl };
			}

			if (result.RequiresTwoFactor)
			{
				return new ApiResult<ExternalRegisterModel> { Success = false, Message = await _localizer.Get<Login.Resources>(x => x.RequiresTwoFactor, cancellationToken) };
			}

			if (result.IsLockedOut)
			{
				_logger.LogWarning("User account locked out.");

				return new ApiResult<ExternalRegisterModel> { Success = false, Message = await _localizer.Get<Login.Resources>(x => x.IsLockedOut, cancellationToken) };
			}

			if (result.IsNotAllowed)
			{
				return new ApiResult<ExternalRegisterModel> { Success = false, Message = await _localizer.Get<Login.Resources>(x => x.IsNotAllowed, cancellationToken) };
			}

			// If the user does not have an account, then ask the user to create an account.
			return new ApiResult<ExternalRegisterModel>
			{
				Data = new ExternalRegisterModel
				{
					Provider = info.LoginProvider,
					ReturnUrl = request.ReturnUrl,
					Email = info.Principal.FindFirstValue(ClaimTypes.Email),
					FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
					LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
				}
			};
		}
	}
}
