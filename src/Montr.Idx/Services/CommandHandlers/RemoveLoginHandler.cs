using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Entities;
using Montr.Idx.Services.Implementations;

namespace Montr.Idx.Services.CommandHandlers
{
	public class RemoveLoginHandler : IRequestHandler<RemoveLogin, ApiResult>
	{
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;

		public RemoveLoginHandler(
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<ApiResult> Handle(RemoveLogin request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);
			if (user == null)
			{
				return null;
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var result = await _userManager.RemoveLoginAsync(user, request.LoginProvider, request.ProviderKey);
			if (result.Succeeded == false)
			{
				// StatusMessage = "The external login was not removed.";
				return result.ToApiResult();
			}

			await _signInManager.RefreshSignInAsync(user);

			// StatusMessage = "The external login was removed.";
			return new ApiResult();
		}
	}
}
