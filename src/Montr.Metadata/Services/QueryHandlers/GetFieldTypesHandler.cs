using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Metadata.Models;
using Montr.Metadata.Queries;

namespace Montr.Metadata.Services.QueryHandlers
{
	public class GetFieldTypesHandler : IRequestHandler<GetFieldTypes, IList<FieldType>>
	{
		private readonly IFieldProviderRegistry _fieldProviderRegistry;

		public GetFieldTypesHandler(IFieldProviderRegistry fieldProviderRegistry)
		{
			_fieldProviderRegistry = fieldProviderRegistry;
		}

		public async Task<IList<FieldType>> Handle(GetFieldTypes request, CancellationToken cancellationToken)
		{
			var result = _fieldProviderRegistry.GetFieldTypes();

			return await Task.FromResult(result);
		}
	}
}
