using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Messages.Services;

namespace Montr.Automate.Impl.Services
{
	public class NotifyByEmailAutomationActionProvider : IAutomationActionProvider
	{
		private readonly IEmailSender _emailSender;

		public NotifyByEmailAutomationActionProvider(IEmailSender emailSender)
		{
			_emailSender = emailSender;
		}

		public async Task Execute(AutomationAction automationAction, object entity, CancellationToken cancellationToken)
		{
			var action = (NotifyByEmailAutomationAction)automationAction;

			await _emailSender.Send(action.Recipient, action.Subject, action.Body, cancellationToken);
		}
	}
}
