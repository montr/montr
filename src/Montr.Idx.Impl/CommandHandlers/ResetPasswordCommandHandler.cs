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
	public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ApiResult>
	{
		private readonly ILogger<ResetPasswordCommandHandler> _logger;
		private readonly UserManager<DbUser> _userManager;

		public ResetPasswordCommandHandler(
			ILogger<ResetPasswordCommandHandler> logger,
			UserManager<DbUser> userManager)
		{
			_logger = logger;
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user != null)
			{
				var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));

				var identityResult = await _userManager.ResetPasswordAsync(user, code, request.Password);

				if (identityResult.Succeeded == false)
				{
					return identityResult.ToApiResult();
				}

				// todo: add email notification
			}

			// Your password has been reset. Please <a asp-page="./Login">click here to log in</a>.
			return new ApiResult();
		}
	}
}
