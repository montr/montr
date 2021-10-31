using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Queries;
using Montr.Automate.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Impl.QueryHandlers
{
	public class GetAutomationMetadataHandler : IRequestHandler<GetAutomationMetadata, IList<FieldMetadata>>
	{
		private readonly IAutomationProviderRegistry _providerRegistry;

		public GetAutomationMetadataHandler(IAutomationProviderRegistry providerRegistry)
		{
			_providerRegistry = providerRegistry;
		}

		public Task<IList<FieldMetadata>> Handle(GetAutomationMetadata request, CancellationToken cancellationToken)
		{
			IList<FieldMetadata> metadata = null;

			if (request.ActionTypeCode != null)
			{
				var actionProvider = _providerRegistry.GetActionProvider(request.ActionTypeCode);

				metadata = actionProvider.GetMetadata();
			}
			else if (request.ConditionTypeCode != null)
			{
				var conditionProvider = _providerRegistry.GetConditionProvider(request.ConditionTypeCode);

				metadata = conditionProvider.GetMetadata();
			}

			return Task.FromResult(metadata);
		}
	}
}
