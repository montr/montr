using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Models;
using Montr.Automate.Queries;
using Montr.Metadata.Models;

namespace Montr.Automate.Services.QueryHandlers
{
	public class GetAutomationConditionMetadataHandler : IRequestHandler<GetAutomationConditionMetadata, IList<FieldMetadata>>
	{
		private readonly IAutomationProviderRegistry _providerRegistry;

		public GetAutomationConditionMetadataHandler(IAutomationProviderRegistry providerRegistry)
		{
			_providerRegistry = providerRegistry;
		}

		public async Task<IList<FieldMetadata>> Handle(GetAutomationConditionMetadata request, CancellationToken cancellationToken)
		{
			var context = new AutomationContext { EntityTypeCode = request.EntityTypeCode };

			var provider = _providerRegistry.GetConditionProvider(request.Condition.Type);

			return await provider.GetMetadata(context, request.Condition, cancellationToken);
		}
	}
}
