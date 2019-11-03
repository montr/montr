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
	public class ExternalLoginCallbackCommandHandler : IRequestHandler<ExternalLoginCallbackCommand, ExternalLoginCallbackCommand.Result>
	{
		private readonly ILogger<ExternalLoginCallbackCommandHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;

		public ExternalLoginCallbackCommandHandler(
			ILogger<ExternalLoginCallbackCommandHandler> logger,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_signInManager = signInManager;
		}

		public async Task<ExternalLoginCallbackCommand.Result> Handle(ExternalLoginCallbackCommand request, CancellationToken cancellationToken)
		{
			if (request.RemoteError != null)
			{
				return new ExternalLoginCallbackCommand.Result($"Error from external provider: {request.RemoteError}");
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return new ExternalLoginCallbackCommand.Result("Error loading external login information.");
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
			if (result.Succeeded)
			{
				_logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);

				return new ExternalLoginCallbackCommand.Result { RedirectUrl = request.ReturnUrl };
			}

			// todo: implement
			if (result.IsNotAllowed || result.RequiresTwoFactor)
			{
			}

			if (result.IsLockedOut)
			{
				return new ExternalLoginCallbackCommand.Result("This account has been locked out, please try again later.");
			}

			// If the user does not have an account, then ask the user to create an account.
			return new ExternalLoginCallbackCommand.Result
			{
				Register = new ExternalRegisterUserModel
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
