using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResult>
	{
		private readonly ILogger<RegisterUserCommandHandler> _logger;
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly IAppUrlBuilder _appUrlBuilder;
		private readonly EmailConfirmationService _emailConfirmationService;

		public RegisterUserCommandHandler(
			ILogger<RegisterUserCommandHandler> logger,
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager,
			IAppUrlBuilder appUrlBuilder,
			EmailConfirmationService emailConfirmationService)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_appUrlBuilder = appUrlBuilder;

			_emailConfirmationService = emailConfirmationService;
		}

		public async Task<ApiResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
		{

			var user = new DbUser
			{
				Id = Guid.NewGuid(),
				UserName = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email
			};

			var identityResult = await _userManager.CreateAsync(user, request.Password);

			if (identityResult.Succeeded)
			{
				_logger.LogInformation("User created a new account with password.");

				await _emailConfirmationService.SendMessage(user, cancellationToken);

				if (_userManager.Options.SignIn.RequireConfirmedAccount)
				{
					// return RedirectToPage("RegisterConfirmation", new { email = request.Email });

					var redirectUrl = _appUrlBuilder.Build(ClientRoutes.RegisterConfirmation,
						new Dictionary<string, string> { { "email", request.Email } });

					return new ApiResult { Success = true, RedirectUrl = redirectUrl };
				}

				await _signInManager.SignInAsync(user, isPersistent: false);

				return new ApiResult { Success = true, RedirectUrl = request.ReturnUrl };
			}

			return identityResult.ToApiResult();
		}
	}
}
