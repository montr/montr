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
	public class GetFieldAutomationConditionFieldListHandler : IRequestHandler<GetFieldAutomationConditionFieldList, IList<FieldMetadata>>
	{
		private readonly IAutomationContextProvider _automationContextProvider;

		public GetFieldAutomationConditionFieldListHandler(IAutomationContextProvider automationContextProvider)
		{
			_automationContextProvider = automationContextProvider;
		}

		public async Task<IList<FieldMetadata>> Handle(GetFieldAutomationConditionFieldList request, CancellationToken cancellationToken)
		{
			return await _automationContextProvider.GetFields(new AutomationContext
			{
				MetadataEntityTypeCode = request.EntityTypeCode,
				MetadataEntityUid = request.EntityTypeUid
			}, cancellationToken);
		}
	}
}
