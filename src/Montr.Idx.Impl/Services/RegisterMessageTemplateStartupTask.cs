using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Idx.Impl.CommandHandlers;
using Montr.Messages.Commands;
using Montr.Messages.Models;
using Montr.Messages.Services;

namespace Montr.Idx.Impl.Services
{
	public class RegisterMessageTemplateStartupTask : AbstractRegisterMessageTemplateStartupTask
	{
		public RegisterMessageTemplateStartupTask(ILogger<RegisterMessageTemplateStartupTask> logger, IMediator mediator) : base(logger, mediator)
		{
		}

		protected override IEnumerable<RegisterMessageTemplate> GetCommands()
		{
			yield return new RegisterMessageTemplate
			{
				Item = new MessageTemplate
				{
					Uid = EmailConfirmationService.TemplateUid,
					Subject = "📧 Confirm your email",
					Body = @"
### Hello!

Please confirm your account by clicking here <{{CallbackUrl}}>.
"
				}
			};

			yield return new RegisterMessageTemplate
			{
				Item = new MessageTemplate
				{
					Uid = ForgotPasswordHandler.TemplateUid,
					Subject = "❗ Reset Password",
					Body = @"
### Hello!

Please reset your password by clicking here <{{CallbackUrl}}>.
"
				}
			};
		}
	}
}
