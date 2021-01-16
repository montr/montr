using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Models;
using Montr.Idx.Services;
using Montr.Kompany.Commands;
using Montr.Kompany.Models;

namespace Montr.Kompany.Impl.CommandHandlers
{
	public class SetupSystemHandler : IRequestHandler<SetupSystem, ApiResult>
	{
		private readonly ILogger<SetupSystemHandler> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IUserManager _userManager;
		private readonly ISignInManager _signInManager;
		private readonly ISettingsRepository _settingsRepository;
		private readonly IAppUrlBuilder _appUrlBuilder;
		private readonly ISender _mediator;

		public SetupSystemHandler(ILogger<SetupSystemHandler> logger,
			IUnitOfWorkFactory unitOfWorkFactory,
			IUserManager userManager,
			ISignInManager signInManager,
			ISettingsRepository settingsRepository,
			IAppUrlBuilder appUrlBuilder,
			ISender mediator)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_userManager = userManager;
			_signInManager = signInManager;
			_settingsRepository = settingsRepository;

			_appUrlBuilder = appUrlBuilder;
			_mediator = mediator;
		}

		public async Task<ApiResult> Handle(SetupSystem request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				_logger.LogInformation($"Creating default administrator {request.AdminEmail}", request.AdminEmail);

				var user = new User
				{
					UserName = request.AdminEmail,
					Email = request.AdminEmail
				};

				var userResult = await _userManager.CreateUser(user, request.AdminPassword, cancellationToken);

				if (userResult.Success == false) return userResult;

				var signInResult = await _signInManager.PasswordSignIn(user.UserName, request.AdminPassword, true, false);

				if (signInResult.Success == false) return signInResult;

				_logger.LogInformation($"Creating default company {request.CompanyName}", request.CompanyName);

				// todo: create company without mediator and company request
				var companyResult = await _mediator.Send(new CreateCompany
				{
					// ReSharper disable once PossibleInvalidOperationException
					UserUid = userResult.Uid.Value,
					Item = new Company
					{
						Name = request.CompanyName,
						ConfigCode = CompanyConfigCode.Company // todo: register and use allowed company types
					}
				}, cancellationToken);

				if (companyResult.Success == false) return companyResult;

				await _settingsRepository.GetSettings<AppOptions>()
					.Set(x => x.State, AppState.Initialized)
					.Update(cancellationToken);

				scope.Commit();

				return new ApiResult { RedirectUrl = _appUrlBuilder.Build(ClientRoutes.Dashboard) };
			}
		}
	}
}
