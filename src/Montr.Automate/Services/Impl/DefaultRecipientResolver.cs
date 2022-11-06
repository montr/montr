using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Core.Services;

namespace Montr.Automate.Services.Impl
{
	public class DefaultRecipientResolver : IRecipientResolver
	{
		public class KnownTypes
		{
			public const string CurrentUser = "current-user";

			public const string Operator = "operator";
		}

		private readonly INamedServiceFactory<IRecipientResolver> _recipientResolverFactory;

		public DefaultRecipientResolver(INamedServiceFactory<IRecipientResolver> recipientResolverFactory)
		{
			_recipientResolverFactory = recipientResolverFactory;
		}

		public async Task<Recipient> Resolve(string recipient, AutomationContext context, CancellationToken cancellationToken)
		{
			var recipientResolver = _recipientResolverFactory.GetRequiredService(context.EntityTypeCode);

			var result = await recipientResolver.Resolve(recipient, context, cancellationToken);

			if (result != null) return result;

			// todo: resolve system recipients (current-user, operator)
			if (recipient == KnownTypes.CurrentUser || recipient == KnownTypes.Operator)
			{
				return new Recipient { Email = recipient };
			}

			return null;
		}
	}
}
