﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Models;
using Montr.Idx.Services;
using Montr.Kompany.Commands;
using Montr.Kompany.Models;
using Montr.MasterData.Services;
using ClientRoutes = Montr.Core.ClientRoutes;

namespace Montr.Kompany.Impl.CommandHandlers
{
	public class SetupSystemHandler : IRequestHandler<SetupSystem, ApiResult>
	{
		private readonly ILogger<SetupSystemHandler> _logger;
		private readonly IOptionsMonitor<AppOptions> _optionsMonitor;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly ISignInManager _signInManager;
		private readonly ISettingsRepository _settingsRepository;
		private readonly IAppUrlBuilder _appUrlBuilder;
		private readonly ILocalizer _localizer;
		private readonly ISender _mediator;

		public SetupSystemHandler(ILogger<SetupSystemHandler> logger,
			IOptionsMonitor<AppOptions> optionsMonitor,
			IUnitOfWorkFactory unitOfWorkFactory,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			ISignInManager signInManager,
			ISettingsRepository settingsRepository,
			IAppUrlBuilder appUrlBuilder,
			ILocalizer localizer,
			ISender mediator)
		{
			_logger = logger;
			_optionsMonitor = optionsMonitor;
			_unitOfWorkFactory = unitOfWorkFactory;
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_signInManager = signInManager;
			_settingsRepository = settingsRepository;
			_appUrlBuilder = appUrlBuilder;
			_localizer = localizer;
			_mediator = mediator;
		}

		public async Task<ApiResult> Handle(SetupSystem request, CancellationToken cancellationToken)
		{
			var appOptions = _optionsMonitor.CurrentValue;

			if (appOptions.State == AppState.Initialized)
			{
				return new ApiResult
				{
					Success = false,
					Message = await _localizer.Get("page.setup.initializedMessage", cancellationToken)
				};
			}

			using (var scope = _unitOfWorkFactory.Create())
			{
				_logger.LogInformation($"Creating default administrator {request.AdminEmail}", request.AdminEmail);

				var user = new User
				{
					Name = request.AdminEmail,
					UserName = request.AdminEmail,
					Password = request.AdminPassword,
					Email = request.AdminEmail
				};

				var userRepository = _classifierRepositoryFactory.GetRequiredService(Idx.ClassifierTypeCode.User);

				var userResult = await userRepository.Insert(user, cancellationToken);

				if (userResult.Success == false) return userResult;

				var signInResult = await _signInManager.PasswordSignIn(user.UserName, request.AdminPassword, true, false);

				if (signInResult.Success == false) return signInResult;

				var userUid = userResult.Uid;

				_logger.LogInformation($"Creating default company {request.CompanyName}", request.CompanyName);

				// todo: create company without mediator and company request
				var companyResult = await _mediator.Send(new CreateCompany
				{
					UserUid = userUid,
					Item = new Company
					{
						Name = request.CompanyName,
						ConfigCode = CompanyConfigCode.Company // todo: register and use allowed company types
					}
				}, cancellationToken);

				if (companyResult.Success == false) return companyResult;

				_logger.LogInformation("Updating application initialization options");

				await _settingsRepository.GetSettings<AppOptions>()
					.Set(x => x.State, AppState.Initialized)
					.Set(x => x.SuperUserId, userUid)
					.Update(cancellationToken);

				scope.Commit();

				return new ApiResult { RedirectUrl = _appUrlBuilder.Build(ClientRoutes.Dashboard) };
			}
		}
	}
}
