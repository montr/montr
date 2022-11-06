using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Entities;

namespace Montr.Idx.Services.CommandHandlers
{
	public class ConfirmEmailHandler : IRequestHandler<ConfirmEmail, ApiResult>
	{
		private readonly ILogger<ConfirmEmailHandler> _logger;
		private readonly UserManager<DbUser> _userManager;

		public ConfirmEmailHandler(
			ILogger<ConfirmEmailHandler> logger,
			UserManager<DbUser> userManager)
		{
			_logger = logger;
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(ConfirmEmail request, CancellationToken cancellationToken)
		{
			if (request.UserId == null || request.Code == null)
			{
				// return RedirectToPage("/Index");
				return new ApiResult { Success = false };
			}

			// todo: move to EmailConfirmationService
			var user = await _userManager.FindByIdAsync(request.UserId);

			if (user == null)
			{
				// $"Unable to load user with ID '{userId}'."
				return new ApiResult { Success = false };
			}

			var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

			var result = await _userManager.ConfirmEmailAsync(user, code);

			if (result.Succeeded)
			{
				// Thank you for confirming your email.
				return new ApiResult();
			}

			return new ApiResult
			{
				Success = false,
				Errors = new[]
				{
					new ApiResultError { Messages = new [] { "Error confirming your email." }}, 
				}
			};
		}
	}
}
