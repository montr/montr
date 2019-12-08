﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Montr.Core.Services;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Messages.Services;

namespace Montr.Idx.Impl.Services
{
	public class EmailConfirmationService
	{
		private readonly UserManager<DbUser> _userManager;
		private readonly IAppUrlBuilder _appUrlBuilder;
		private readonly IEmailSender _emailSender;
		private readonly ITemplateRenderer _templateRenderer;

		public EmailConfirmationService(
			UserManager<DbUser> userManager,
			IAppUrlBuilder appUrlBuilder,
			IEmailSender emailSender,
			ITemplateRenderer templateRenderer)
		{
			_userManager = userManager;
			_appUrlBuilder = appUrlBuilder;
			_emailSender = emailSender;
			_templateRenderer = templateRenderer;
		}

		public async Task SendConfirmEmailMessage(DbUser user, CancellationToken cancellationToken)
		{
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			var messageModel = new ConfirmEmailMessageModel
			{
				CallbackUrl = _appUrlBuilder.Build($"{ClientRoutes.ConfirmEmail}/{user.Id}/{code}")
			};

			var templateUid = Guid.Parse("CEEF2983-C083-448F-88B1-2DA6E6CB41A4");

			var message = await _templateRenderer.Render(templateUid, messageModel, cancellationToken);

			await _emailSender.Send(user.Email, message.Subject, message.Body);
		}

		public async Task SendConfirmEmailChangeMessage(DbUser user, string newEmail, CancellationToken cancellationToken)
		{
			var code = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			var messageModel = new ConfirmEmailMessageModel
			{
				CallbackUrl = _appUrlBuilder.Build($"{ClientRoutes.ConfirmEmailChange}/{user.Id}/{newEmail}/{code}")
			};

			var templateUid = Guid.Parse("CEEF2983-C083-448F-88B1-2DA6E6CB41A4");

			var message = await _templateRenderer.Render(templateUid, messageModel, cancellationToken);

			await _emailSender.Send(newEmail, message.Subject, message.Body);
		}
	}
}
