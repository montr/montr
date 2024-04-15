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
using Montr.Idx.Entities;
using Montr.Idx.Models;
using Montr.Idx.Services.Implementations;
using Montr.MasterData.Services;

namespace Montr.Idx.Services.CommandHandlers
{
	public class RegisterHandler : IRequestHandler<Register, ApiResult>
	{
		private readonly ILogger<RegisterHandler> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly IAppUrlBuilder _appUrlBuilder;
		private readonly IEmailConfirmationService _emailConfirmationService;

		public RegisterHandler(
			ILogger<RegisterHandler> logger,
			IUnitOfWorkFactory unitOfWorkFactory,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager,
			IAppUrlBuilder appUrlBuilder,
			IEmailConfirmationService emailConfirmationService)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_userManager = userManager;
			_signInManager = signInManager;
			_appUrlBuilder = appUrlBuilder;
			_emailConfirmationService = emailConfirmationService;
		}

		public async Task<ApiResult> Handle(Register request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var user = new User
			{
				Name = request.Email,
				UserName = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email,
				Password = request.Password
			};

			using (var scope = _unitOfWorkFactory.Create())
			{
				var userRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.User);

				var result = await userRepository.Insert(user, cancellationToken);

				if (result.Success)
				{
					_logger.LogInformation("User created a new account with password.");

					// todo: remove reload user
					var dbUser = await _userManager.FindByIdAsync(result.Uid.ToString())
					             ?? throw new InvalidOperationException(
						             $"Created user with uid {result.Uid} not found.");

					if (_userManager.Options.SignIn.RequireConfirmedEmail)
					{
						await _emailConfirmationService.SendConfirmEmailMessage(dbUser, cancellationToken);
					}

					// todo: add user default roles

					scope.Commit();

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						var redirectUrl = _appUrlBuilder.Build(ClientRoutes.RegisterConfirmation,
							new Dictionary<string, string> { { "email", request.Email } });

						return new ApiResult { RedirectUrl = redirectUrl };
					}

					await _signInManager.SignInAsync(dbUser, isPersistent: false);

					return new ApiResult { RedirectUrl = request.ReturnUrl };
				}

				return result;
			}
		}
	}
}
