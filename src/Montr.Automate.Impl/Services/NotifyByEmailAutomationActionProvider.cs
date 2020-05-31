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
		private readonly ITemplateRenderer _templateRenderer;

		public NotifyByEmailAutomationActionProvider(IEmailSender emailSender, ITemplateRenderer templateRenderer)
		{
			_emailSender = emailSender;
			_templateRenderer = templateRenderer;
		}

		public async Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
		{
			var action = (NotifyByEmailAutomationAction)automationAction;

			var message = await _templateRenderer.Render(action.Subject, action.Body, context.Entity, cancellationToken);

			await _emailSender.Send(action.Recipient, message.Subject, message.Body, cancellationToken);
		}
	}
}
