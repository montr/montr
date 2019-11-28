using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class SendEmailConfirmationHandler : IRequestHandler<SendEmailConfirmation, ApiResult>
	{
		private readonly ILocalizer _localizer;
		private readonly UserManager<DbUser> _userManager;
		private readonly EmailConfirmationService _emailConfirmationService;

		public SendEmailConfirmationHandler(
			ILocalizer localizer,
			UserManager<DbUser> userManager,
			EmailConfirmationService emailConfirmationService)
		{
			_localizer = localizer;
			_userManager = userManager;
			_emailConfirmationService = emailConfirmationService;
		}

		public async Task<ApiResult> Handle(SendEmailConfirmation request, CancellationToken cancellationToken)
		{
			var user = request.User != null
				? await _userManager.GetUserAsync(request.User)
				: await _userManager.FindByEmailAsync(request.Email);

			if (user != null)
			{
				await _emailConfirmationService.SendConfirmEmailMessage(user, cancellationToken);
			}

			return new ApiResult { Message = await _localizer.Get<SendEmailConfirmation.Resources>(x => x.Success, cancellationToken) };
		}
	}
}
