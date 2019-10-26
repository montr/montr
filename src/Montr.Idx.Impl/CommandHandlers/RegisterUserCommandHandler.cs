using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
	public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResult>
	{
		private readonly ILogger<RegisterUserCommandHandler> _logger;
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly IAppUrlBuilder _appUrlBuilder;
		private readonly IEmailSender _emailSender;
		private readonly ITemplateRenderer _templateRenderer;

		public RegisterUserCommandHandler(
			ILogger<RegisterUserCommandHandler> logger,
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager,
			IAppUrlBuilder appUrlBuilder,
			IEmailSender emailSender,
			ITemplateRenderer templateRenderer)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_appUrlBuilder = appUrlBuilder;
			_emailSender = emailSender;
			_templateRenderer = templateRenderer;
		}

		public async Task<ApiResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
		{
			var model = (RegisterUserModel)request;
			var validationResults = new List<ValidationResult>();

			if (Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true))
			{
				var user = new DbUser
				{
					Id = Guid.NewGuid(),
					UserName = request.Email,
					FirstName = request.FirstName,
					LastName = request.LastName,
					Email = request.Email
				};

				var identityResult = await _userManager.CreateAsync(user, request.Password);

				if (identityResult.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					await SendEmailConfirmationMessage(user, cancellationToken);

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						// return RedirectToPage("RegisterConfirmation", new { email = request.Email });

						var redirectUrl = _appUrlBuilder.Build(ClientRoutes.RegisterConfirmation,
							new Dictionary<string, string> { {"email", request.Email} });

						return new ApiResult { Success = true, RedirectUrl = redirectUrl };
					}

					await _signInManager.SignInAsync(user, isPersistent: false);

					return new ApiResult { Success = true, RedirectUrl = request.ReturnUrl };
				}

				return new ApiResult
				{
					Success = false,
					Errors = identityResult.Errors
						.Select(x => new ApiResultError
						{
							Key = x.Code,
							Messages = new[] { x.Description }
						}).ToArray()
				};
			}

			return new ApiResult
			{
				Success = false,
				Errors = new[]
				{
					new ApiResultError
					{
						Messages = validationResults.Select(x => x.ErrorMessage).ToArray()
					}
				}
			};
		}

		private async Task SendEmailConfirmationMessage(DbUser user, CancellationToken cancellationToken)
		{
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			var callbackUrl = _appUrlBuilder.Build(ClientRoutes.ConfirmEmail,
				new Dictionary<string, string>
				{
					{"userId", user.Id.ToString()},
					{"code", code}
				});

			var messageModel = new ConfirmEmailMessageModel
			{
				CallbackUrl = callbackUrl 
			};

			var templateUid = Guid.Parse("CEEF2983-C083-448F-88B1-2DA6E6CB41A4");

			var message = await _templateRenderer.Render(templateUid, messageModel, cancellationToken);

			// $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>."
			await _emailSender.Send(user.Email, message.Subject, message.Body);
		}
	}
}
