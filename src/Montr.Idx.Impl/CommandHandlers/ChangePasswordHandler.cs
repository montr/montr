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
	public class ChangePasswordHandler : IRequestHandler<ChangePassword, ApiResult>
	{
		private readonly ILogger<ChangePasswordHandler> _logger;
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;

		public ChangePasswordHandler(
			ILogger<ChangePasswordHandler> logger,
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<ApiResult> Handle(ChangePassword request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);

			if (user == null)
			{
				return new ApiResult { Success = false };
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var identityResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
			if (identityResult.Succeeded == false)
			{
				return identityResult.ToApiResult();
			}

			await _signInManager.RefreshSignInAsync(user);

			_logger.LogInformation("User changed their password successfully.");

			// StatusMessage = "Your password has been changed.";
			return new ApiResult();
		}
	}
}
