using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Messages.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class ForgotPasswordHandler : IRequestHandler<ForgotPassword, ApiResult>
	{
		private readonly ILogger<ForgotPasswordHandler> _logger;
		private readonly UserManager<DbUser> _userManager;
		private readonly IAppUrlBuilder _appUrlBuilder;
		private readonly IEmailSender _emailSender;
		private readonly ITemplateRenderer _templateRenderer;

		public ForgotPasswordHandler(
			ILogger<ForgotPasswordHandler> logger,
			UserManager<DbUser> userManager,
			IAppUrlBuilder appUrlBuilder,
			IEmailSender emailSender,
			ITemplateRenderer templateRenderer)
		{
			_logger = logger;
			_userManager = userManager;
			_appUrlBuilder = appUrlBuilder;
			_emailSender = emailSender;
			_templateRenderer = templateRenderer;
		}

		public async Task<ApiResult> Handle(ForgotPassword request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user != null && await _userManager.IsEmailConfirmedAsync(user))
			{
				var code = await _userManager.GeneratePasswordResetTokenAsync(user);

				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

				var messageModel = new ConfirmEmailMessageModel
				{
					CallbackUrl = _appUrlBuilder.Build($"{ClientRoutes.ResetPassword}/{code}")
				};

				var templateUid = Guid.Parse("34ED7F4F-7C6F-44A4-8FA6-2C6F38AB69E0");

				var message = await _templateRenderer.Render(templateUid, messageModel, cancellationToken);

				await _emailSender.Send(user.Email, message.Subject, message.Body);
			}

			// Please check your email to reset your password.
			// return RedirectToPage("./ForgotPasswordConfirmation");
			return new ApiResult();
		}
	}
}
