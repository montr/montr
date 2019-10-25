using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Messages.Services;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResult>
	{
		private readonly ILogger<RegisterUserCommandHandler> _logger;
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;
		private readonly IEmailSender _emailSender;

		public RegisterUserCommandHandler(
			ILogger<RegisterUserCommandHandler> logger,
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager,
			IEmailSender emailSender)
		{
			_logger = logger;
			_userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
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

					var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

					var callbackUrl = "/Account/ConfirmEmail";

					/*var callbackUrl = Url.Page(
						"/Account/ConfirmEmail",
						pageHandler: null,
						values: new { area = "Identity", userId = user.Id, code = code },
						protocol: Request.Scheme);*/

					// todo: use message template
					await _emailSender.Send(request.Email, "Confirm your email",
						$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						// return RedirectToPage("RegisterConfirmation", new { email = request.Email });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);

						// return LocalRedirect(returnUrl);
					}

					return new ApiResult { Success = true };
				}

				return new ApiResult
				{
					Success = false,
					Errors = identityResult.Errors.Select(x =>
						new ApiResultError
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
	}
}
