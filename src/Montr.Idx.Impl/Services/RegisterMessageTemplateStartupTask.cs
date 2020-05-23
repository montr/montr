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
				Subject = "📧 Confirm your email",
				Body = @"
### Hello!

Please confirm your account by clicking here <{{CallbackUrl}}>.
"
			};

			yield return new MessageTemplate
			{
				Uid = ForgotPasswordHandler.TemplateUid,
				Subject = "❗ Reset Password",
				Body = @"
### Hello!

Please reset your password by clicking here <{{CallbackUrl}}>.
"
			};
		}
	}
}
