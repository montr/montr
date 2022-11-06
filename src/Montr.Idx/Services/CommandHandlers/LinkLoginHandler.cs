using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Entities;

namespace Montr.Idx.Services.CommandHandlers
{
	public class LinkLoginHandler : IRequestHandler<LinkLogin, ChallengeResult>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly UserManager<DbUser> _userManager;
		private readonly IAppUrlBuilder _appUrlBuilder;

		public LinkLoginHandler(
			IHttpContextAccessor httpContextAccessor,
			SignInManager<DbUser> signInManager,
			UserManager<DbUser> userManager,
			IAppUrlBuilder appUrlBuilder)
		{
			_httpContextAccessor = httpContextAccessor;
			_signInManager = signInManager;
			_userManager = userManager;
			_appUrlBuilder = appUrlBuilder;
		}

		public async Task<ChallengeResult> Handle(LinkLogin request, CancellationToken cancellationToken)
		{
			// Clear the existing external cookie to ensure a clean login process
			await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			// Request a redirect to the external login provider to link a login for the current user
			var redirectUrl = _appUrlBuilder.Build(ClientRoutes.LinkLogin);

			var properties = _signInManager.ConfigureExternalAuthenticationProperties(request.Provider, redirectUrl, _userManager.GetUserId(request.User));

			return new ChallengeResult(request.Provider, properties);
		}
	}
}
