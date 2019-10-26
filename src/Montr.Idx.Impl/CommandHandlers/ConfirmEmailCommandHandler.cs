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

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, ApiResult>
	{
		private readonly ILogger<ConfirmEmailCommandHandler> _logger;
		private readonly UserManager<DbUser> _userManager;

		public ConfirmEmailCommandHandler(
			ILogger<ConfirmEmailCommandHandler> logger,
			UserManager<DbUser> userManager)
		{
			_logger = logger;
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
		{
			if (request.UserId == null || request.Code == null)
			{
				// return RedirectToPage("/Index");
				return new ApiResult { Success = false };
			}

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
