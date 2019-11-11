using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class SetPasswordHandler : IRequestHandler<SetPassword, ApiResult>
	{
		private readonly ILogger<SetPasswordHandler> _logger;
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;

		public SetPasswordHandler(
			ILogger<SetPasswordHandler> logger,
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<ApiResult> Handle(SetPassword request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);
			if (user == null)
			{
				return new ApiResult { Success = false };
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var identityResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
			if (identityResult.Succeeded == false)
			{
				return identityResult.ToApiResult();
			}

			await _signInManager.RefreshSignInAsync(user);

			// StatusMessage = "Your password has been set.";
			return new ApiResult();
		}
	}
}
