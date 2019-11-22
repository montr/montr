using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ConfirmEmailChangeHandler : IRequestHandler<ConfirmEmailChange, ApiResult>
	{
		private readonly ILogger<ConfirmEmailChangeHandler> _logger;
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;

		public ConfirmEmailChangeHandler(
			ILogger<ConfirmEmailChangeHandler> logger,
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		// todo: use transaction
		public async Task<ApiResult> Handle(ConfirmEmailChange request, CancellationToken cancellationToken)
		{
			if (request.UserId == null || request.Email == null || request.Code == null)
			{
				// return RedirectToPage("/Index");
				return new ApiResult { Success = false };
			}

			var user = await _userManager.FindByIdAsync(request.UserId);
			if (user == null)
			{
				// return NotFound($"Unable to load user with ID '{userId}'.");
				return new ApiResult { Success = false };
			}

			var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

			var result = await _userManager.ChangeEmailAsync(user, request.Email, code);

			if (result.Succeeded == false)
			{
				// StatusMessage = "Error changing email.";
				return result.ToApiResult();
			}

			// In our UI email and user name are one and the same, so when we update the email
			// we need to update the user name.
			var setUserNameResult = await _userManager.SetUserNameAsync(user, request.Email);
			if (!setUserNameResult.Succeeded)
			{
				// StatusMessage = "Error changing user name.";
				return setUserNameResult.ToApiResult();
			}

			await _signInManager.RefreshSignInAsync(user);

			// StatusMessage = "Thank you for confirming your email change.";
			return new ApiResult();
		}
	}
}
