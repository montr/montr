using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ChangeEmailHandler : IRequestHandler<ChangeEmail, ApiResult>
	{
		private readonly UserManager<DbUser> _userManager;
		private readonly IEmailConfirmationService _emailConfirmationService;

		public ChangeEmailHandler(
			UserManager<DbUser> userManager,
			IEmailConfirmationService emailConfirmationService)
		{
			_userManager = userManager;
			_emailConfirmationService = emailConfirmationService;
		}

		public async Task<ApiResult> Handle(ChangeEmail request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);
			if (user == null)
			{
				return new ApiResult { Success = false };
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var email = await _userManager.GetEmailAsync(user);

			if (request.Email != email)
			{
				await _emailConfirmationService.SendConfirmEmailChangeMessage(user, request.Email, cancellationToken);

				// StatusMessage = "Confirmation link to change email sent. Please check your email.";
				return new ApiResult();
			}

			// StatusMessage = "Your email is unchanged.";
			return new ApiResult();
		}
	}
}
