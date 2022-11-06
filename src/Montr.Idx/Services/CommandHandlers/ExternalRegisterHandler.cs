using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Entities;
using Montr.Idx.Models;
using Montr.Idx.Services.Implementations;
using Montr.MasterData.Services;

namespace Montr.Idx.Services.CommandHandlers
{
	public class ExternalRegisterHandler : IRequestHandler<ExternalRegister, ApiResult>
	{
		private readonly ILogger<ExternalRegisterHandler> _logger;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly UserManager<DbUser> _userManager;
		private readonly IEmailConfirmationService _emailConfirmationService;

		public ExternalRegisterHandler(
			ILogger<ExternalRegisterHandler> logger,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager,
			IEmailConfirmationService emailConfirmationService)
		{
			_logger = logger;
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_userManager = userManager;
			_signInManager = signInManager;
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

			var user = new User
			{
				Name = request.Email,
				UserName = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email
			};

			var userRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.User);

			var result = await userRepository.Insert(user, cancellationToken);

			if (result.Success)
			{
				// todo: remove reload user
				var dbUser = await _userManager.FindByIdAsync(result.Uid.ToString());

				var identityResult = await _userManager.AddLoginAsync(dbUser, info);

				if (identityResult.Succeeded)
				{
					await _signInManager.SignInAsync(dbUser, isPersistent: false);

					_logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

					await _emailConfirmationService.SendConfirmEmailMessage(dbUser, cancellationToken);

					// todo: check redirect is local
					return new ApiResult { RedirectUrl = request.ReturnUrl };
				}
			}

			return result;
		}
	}
}
