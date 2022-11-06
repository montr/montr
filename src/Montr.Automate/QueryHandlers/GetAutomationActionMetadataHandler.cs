using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Models;
using Montr.Automate.Queries;
using Montr.Automate.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Impl.QueryHandlers
{
	public class GetAutomationActionMetadataHandler : IRequestHandler<GetAutomationActionMetadata, IList<FieldMetadata>>
	{
		private readonly IAutomationProviderRegistry _providerRegistry;

		public GetAutomationActionMetadataHandler(IAutomationProviderRegistry providerRegistry)
		{
			_providerRegistry = providerRegistry;
		}

		public async Task<IList<FieldMetadata>> Handle(GetAutomationActionMetadata request, CancellationToken cancellationToken)
		{
			var context = new AutomationContext { EntityTypeCode = request.EntityTypeCode };

			var conditionProvider = _providerRegistry.GetActionProvider(request.Action.Type);

			return await conditionProvider.GetMetadata(context, request.Action, cancellationToken);
		}
	}
}
