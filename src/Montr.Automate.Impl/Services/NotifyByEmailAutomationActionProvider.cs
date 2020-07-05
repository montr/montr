using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Impl.Models;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Messages.Services;

namespace Montr.Automate.Impl.Services
{
	public class NotifyByEmailAutomationActionProvider : IAutomationActionProvider
	{
		private readonly IRecipientResolver _recipientResolver;
		private readonly ITemplateRenderer _templateRenderer;
		private readonly IEmailSender _emailSender;

		public NotifyByEmailAutomationActionProvider(
			IRecipientResolver recipientResolver,
			ITemplateRenderer templateRenderer,
			IEmailSender emailSender)
		{
			_recipientResolver = recipientResolver;
			_templateRenderer = templateRenderer;
			_emailSender = emailSender;
		}

		public AutomationRuleType RuleType => new AutomationRuleType
		{
			Code = NotifyByEmailAutomationAction.TypeCode,
			Name = "Notify By Email",
			Type = typeof(NotifyByEmailAutomationAction)
		};

		public async Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
		{
			var action = (NotifyByEmailAutomationAction)automationAction;

			var props = action.Props;

			var recipient = await _recipientResolver.Resolve(props.Recipient, context, cancellationToken);

			if (recipient != null)
			{
				var message = await _templateRenderer.Render(props.Subject, props.Body, context.Entity, cancellationToken);

				await _emailSender.Send(recipient.Email, message.Subject, message.Body, cancellationToken);
			}
		}
	}
}
