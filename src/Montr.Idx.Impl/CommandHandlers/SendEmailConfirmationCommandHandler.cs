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
	public class SendEmailConfirmationCommandHandler : IRequestHandler<SendEmailConfirmationCommand, ApiResult>
	{
		private readonly ILogger<SendEmailConfirmationCommandHandler> _logger;
		private readonly UserManager<DbUser> _userManager;
		private readonly EmailConfirmationService _emailConfirmationService;

		public SendEmailConfirmationCommandHandler(
			ILogger<SendEmailConfirmationCommandHandler> logger,
			UserManager<DbUser> userManager,
			EmailConfirmationService emailConfirmationService)
		{
			_logger = logger;
			_userManager = userManager;
			_emailConfirmationService = emailConfirmationService;
		}

		public async Task<ApiResult> Handle(SendEmailConfirmationCommand request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user != null)
			{
				// var userId = await _userManager.GetUserIdAsync(user);

				await _emailConfirmationService.SendMessage(user, cancellationToken);
			}

			return new ApiResult
			{
				Errors = new[]
				{
					new ApiResultError { Messages = new [] { "Verification email sent. Please check your email." }},
				}
			};
		}
	}
}
