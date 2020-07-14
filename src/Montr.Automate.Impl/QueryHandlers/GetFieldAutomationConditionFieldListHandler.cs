using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Queries;
using Montr.Core.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Impl.QueryHandlers
{
	public class GetFieldAutomationConditionFieldListHandler : IRequestHandler<GetFieldAutomationConditionFieldList, IList<FieldMetadata>>
	{
		private readonly INamedServiceFactory<IEntityContextProvider> _entityContextProviderFactory;

		public GetFieldAutomationConditionFieldListHandler(INamedServiceFactory<IEntityContextProvider> entityContextProviderFactory)
		{
			_entityContextProviderFactory = entityContextProviderFactory;
		}

		public async Task<IList<FieldMetadata>> Handle(GetFieldAutomationConditionFieldList request, CancellationToken cancellationToken)
		{
			var entityContextProvider = _entityContextProviderFactory.Resolve(request.EntityTypeCode);

			var entityType = entityContextProvider.GetEntityType(request.EntityTypeCode, request.EntityTypeUid);

			var fields = entityType
				.GetProperties()
				.Select(x => new TextField { Key = x.Name, Name = x.Name })
				.Cast<FieldMetadata>()
				.ToList();

			return await Task.FromResult(fields);
		}
	}
}
