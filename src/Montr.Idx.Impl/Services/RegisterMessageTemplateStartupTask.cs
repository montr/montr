using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Idx.Impl.CommandHandlers;
using Montr.Messages.Models;
using Montr.Messages.Services;

namespace Montr.Idx.Impl.Services
{
	public class RegisterMessageTemplateStartupTask : IStartupTask
	{
		private readonly IMessageTemplateRegistrator _registrator;

		public RegisterMessageTemplateStartupTask(IMessageTemplateRegistrator registrator)
		{
			_registrator = registrator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var item in GetMessageTemplates())
			{
				await _registrator.Register(item, cancellationToken);
			}
		}

		protected IEnumerable<MessageTemplate> GetMessageTemplates()
		{
			yield return new MessageTemplate
			{
				Uid = EmailConfirmationService.TemplateUid,
				Code = MessageTemplateCodes.EmailConfirmation,
				Name = "Email confirmation",
				IsSystem = true,
				Subject = "📧 Confirm your email",
				Body = @"
### Hello!

Please confirm your account by clicking here <{{CallbackUrl}}>.
"
			};

			yield return new MessageTemplate
			{
				Uid = ForgotPasswordHandler.TemplateUid,
				Code = MessageTemplateCodes.PasswordReset,
				Name = "Password reset",
				IsSystem = true,
				Subject = "❗ Reset password",
				Body = @"
### Hello!

Please reset your password by clicking here <{{CallbackUrl}}>.
"
			};
		}
	}
}
