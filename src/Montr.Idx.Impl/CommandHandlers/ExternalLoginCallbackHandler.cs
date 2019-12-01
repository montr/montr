using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ExternalLoginCallbackHandler : IRequestHandler<ExternalLoginCallback, ExternalLoginCallback.Result>
	{
		private readonly ILogger<ExternalLoginCallbackHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;

		public ExternalLoginCallbackHandler(
			ILogger<ExternalLoginCallbackHandler> logger,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_signInManager = signInManager;
		}

		public async Task<ExternalLoginCallback.Result> Handle(ExternalLoginCallback request, CancellationToken cancellationToken)
		{
			if (request.RemoteError != null)
			{
				return new ExternalLoginCallback.Result { Success = false, Message = $"Error from external provider: {request.RemoteError}" };
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return new ExternalLoginCallback.Result { Success = false, Message = "Error loading external login information." };
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
			if (result.Succeeded)
			{
				_logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);

				return new ExternalLoginCallback.Result { RedirectUrl = request.ReturnUrl };
			}

			// todo: implement
			if (result.IsNotAllowed || result.RequiresTwoFactor)
			{
			}

			if (result.IsLockedOut)
			{
				return new ExternalLoginCallback.Result { Success = false, Message = "This account has been locked out, please try again later." };
			}

			// If the user does not have an account, then ask the user to create an account.
			return new ExternalLoginCallback.Result
			{
				Register = new ExternalRegisterModel
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
