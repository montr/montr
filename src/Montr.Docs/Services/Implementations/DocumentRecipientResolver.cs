using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Idx.Models;
using Montr.MasterData.Services;

namespace Montr.Docs.Services.Implementations
{
	public class DocumentRecipientResolver : IRecipientResolver
	{
		public class KnownTypes
		{
			public const string Requester = "requester";
		}

		private readonly IAutomationContextProvider _automationContextProvider;
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;

		public DocumentRecipientResolver(IAutomationContextProvider automationContextProvider,
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory)
		{
			_automationContextProvider = automationContextProvider;
			_classifierRepositoryFactory = classifierRepositoryFactory;
		}

		public async Task<Recipient> Resolve(string recipient, AutomationContext context, CancellationToken cancellationToken)
		{
			if (recipient == KnownTypes.Requester)
			{
				var document = (Document) await _automationContextProvider.GetEntity(context, cancellationToken);

				if (document.CreatedBy != null)
				{
					var userRepository = _classifierRepositoryFactory.GetNamedOrDefaultService(Idx.ClassifierTypeCode.User);

					var searchResult = await userRepository.Search(
						new UserSearchRequest { TypeCode = Idx.ClassifierTypeCode.User, Uid = document.CreatedBy }, cancellationToken);

					if (searchResult.Rows.SingleOrDefault() is User user)
					{
						return new Recipient { Email = user.Email };
					}
				}
			}

			return null;
		}
	}
}
