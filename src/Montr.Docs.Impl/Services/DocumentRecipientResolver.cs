using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Idx.Models;

namespace Montr.Docs.Impl.Services
{
	public class DocumentRecipientResolver : IRecipientResolver
	{
		public class KnownTypes
		{
			public const string Requester = "requester";
		}

		private readonly IAutomationContextProvider _automationContextProvider;
		private readonly IRepository<User> _userRepository;

		public DocumentRecipientResolver(IAutomationContextProvider automationContextProvider, IRepository<User> userRepository)
		{
			_automationContextProvider = automationContextProvider;
			_userRepository = userRepository;
		}

		public async Task<Recipient> Resolve(string recipient, AutomationContext context, CancellationToken cancellationToken)
		{
			if (recipient == KnownTypes.Requester)
			{
				var document = (Document) await _automationContextProvider.GetEntity(context, cancellationToken);

				var searchResult = await _userRepository.Search(
					new UserSearchRequest { UserName = document.CreatedBy }, cancellationToken);

				var user = searchResult.Rows.SingleOrDefault();

				if (user != null)
				{
					return new Recipient { Email = user.Email };
				}
			}

			return null;
		}
	}
}
