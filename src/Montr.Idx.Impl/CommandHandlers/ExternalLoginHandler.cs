using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ExternalLoginHandler : IRequestHandler<ExternalLogin, ChallengeResult>
	{
		private readonly SignInManager<DbUser> _signInManager;
		private readonly IAppUrlBuilder _appUrlBuilder;

		public ExternalLoginHandler(
			SignInManager<DbUser> signInManager,
			IAppUrlBuilder appUrlBuilder)
		{
			_signInManager = signInManager;
			_appUrlBuilder = appUrlBuilder;
		}

		public Task<ChallengeResult> Handle(ExternalLogin request, CancellationToken cancellationToken)
		{
			// todo: make returnUrl constant
			var redirectUrl = _appUrlBuilder.Build(ClientRoutes.ExternalLogin,
				new Dictionary<string, string> { { "returnUrl", request.ReturnUrl ?? "/" } });

			var properties = _signInManager.ConfigureExternalAuthenticationProperties(request.Provider, redirectUrl);
			var result = new ChallengeResult(request.Provider, properties);

			return Task.FromResult(result);
		}
	}
}
