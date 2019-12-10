using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class LinkLoginCallbackHandler : IRequestHandler<LinkLoginCallback, ApiResult>
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly UserManager<DbUser> _userManager;

		public LinkLoginCallbackHandler(
			IHttpContextAccessor httpContextAccessor,
			SignInManager<DbUser> signInManager,
			UserManager<DbUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(LinkLoginCallback request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);
			if (user == null)
			{
				return null;
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var info = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
			if (info == null)
			{
				// todo: why user ID displayed to user?
				return new ApiResult { Success = false, Message = $"Unexpected error occurred loading external login info for user with ID '{user.Id}'." };
			}

			var result = await _userManager.AddLoginAsync(user, info);

			if (result.Succeeded == false)
			{
				return result.ToApiResult();
			}

			// Clear the existing external cookie to ensure a clean login process
			await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			return new ApiResult { Message = "The external login was added." };
		}
	}
}
