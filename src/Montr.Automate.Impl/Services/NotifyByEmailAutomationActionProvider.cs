using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Impl.Models;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Messages.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Impl.Services
{
	// todo: move to Messages?
	public class NotifyByEmailAutomationActionProvider : IAutomationActionProvider
	{
		private readonly IAutomationContextProvider _automationContextProvider;
		private readonly IRecipientResolver _recipientResolver;
		private readonly ITemplateRenderer _templateRenderer;
		private readonly IEmailSender _emailSender;

		public NotifyByEmailAutomationActionProvider(
			IAutomationContextProvider automationContextProvider,
			IRecipientResolver recipientResolver,
			ITemplateRenderer templateRenderer,
			IEmailSender emailSender)
		{
			_automationContextProvider = automationContextProvider;
			_recipientResolver = recipientResolver;
			_templateRenderer = templateRenderer;
			_emailSender = emailSender;
		}

		public AutomationRuleType RuleType => new()
		{
			Code = NotifyByEmailAutomationAction.TypeCode,
			Name = "Notify By Email",
			Type = typeof(NotifyByEmailAutomationAction)
		};

		public IList<FieldMetadata> GetMetadata()
		{
			return new List<FieldMetadata>
			{
				new TextField { Key = "recipient", Name = "Recipient", Required = true },
				new TextField { Key = "subject", Name = "Subject", Required = true },
				new TextAreaField { Key = "body", Name = "Body", Required = true, Props = new TextAreaField.Properties { Rows = 2 } }
			};
		}

		public async Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
		{
			var action = (NotifyByEmailAutomationAction)automationAction;

			var props = action.Props;

			var recipient = await _recipientResolver.Resolve(props.Recipient, context, cancellationToken);

			if (recipient != null)
			{
				var entity = await _automationContextProvider.GetEntity(context, cancellationToken);

				var message = await _templateRenderer.Render(props.Subject, props.Body, entity, cancellationToken);

				await _emailSender.Send(recipient.Email, message.Subject, message.Body, cancellationToken);
			}
		}
	}
}
