using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ExternalRegisterHandler : IRequestHandler<ExternalRegister, ApiResult>
	{
		private readonly ILogger<ExternalRegisterHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly UserManager<DbUser> _userManager;
		private readonly IEmailConfirmationService _emailConfirmationService;

		public ExternalRegisterHandler(
			ILogger<ExternalRegisterHandler> logger,
			SignInManager<DbUser> signInManager,
			UserManager<DbUser> userManager,
			IEmailConfirmationService emailConfirmationService)
		{
			_logger = logger;
			_signInManager = signInManager;
			_userManager = userManager;
			_emailConfirmationService = emailConfirmationService;
		}

		public async Task<ApiResult> Handle(ExternalRegister request, CancellationToken cancellationToken)
		{
			// Get the information about the user from the external login provider
			var info = await _signInManager.GetExternalLoginInfoAsync();

			if (info == null)
			{
				// return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
				return new ApiResult { Success = false, Message = "Error loading external login information during confirmation." };
			}

			var user = new DbUser
			{
				Id = Guid.NewGuid(),
				UserName = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email
			};

			// todo: use user repository
			var identityResult = await _userManager.CreateAsync(user);

			if (identityResult.Succeeded)
			{
				identityResult = await _userManager.AddLoginAsync(user, info);

				if (identityResult.Succeeded)
				{
					await _signInManager.SignInAsync(user, isPersistent: false);

					_logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

					await _emailConfirmationService.SendConfirmEmailMessage(user, cancellationToken);

					// todo: check redirect is local
					return new ApiResult { RedirectUrl = request.ReturnUrl };
				}
			}

			return identityResult.ToApiResult();
		}
	}
}
