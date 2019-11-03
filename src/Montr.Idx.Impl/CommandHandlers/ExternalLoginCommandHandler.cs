using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ExternalLoginCommandHandler : IRequestHandler<ExternalLoginCommand, ChallengeResult>
	{
		private readonly ILogger<ExternalLoginCommandHandler> _logger;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly IAppUrlBuilder _appUrlBuilder;

		public ExternalLoginCommandHandler(
			ILogger<ExternalLoginCommandHandler> logger,
			SignInManager<DbUser> signInManager,
			IAppUrlBuilder appUrlBuilder)
		{
			_logger = logger;
			_signInManager = signInManager;
			_appUrlBuilder = appUrlBuilder;
		}

		public Task<ChallengeResult> Handle(ExternalLoginCommand request, CancellationToken cancellationToken)
		{
			var redirectUrl = _appUrlBuilder.Build(ClientRoutes.ExternalLogin,
				new Dictionary<string, string> { { "returnUrl", request.ReturnUrl } });

			var properties = _signInManager.ConfigureExternalAuthenticationProperties(request.Provider, redirectUrl);
			var result = new ChallengeResult(request.Provider, properties);

			return Task.FromResult(result);
		}
	}
}
