using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Messages.Models;

namespace Montr.Idx.Services
{
	public class RegisterMessageTemplateStartupTask : IStartupTask
	{
		private readonly IClassifierRegistrator _registrator;

		public RegisterMessageTemplateStartupTask(IClassifierRegistrator registrator)
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

		protected static IEnumerable<MessageTemplate> GetMessageTemplates()
		{
			yield return new MessageTemplate
			{
				Code = MessageTemplateCode.EmailConfirmation,
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
				Code = MessageTemplateCode.PasswordReset,
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
