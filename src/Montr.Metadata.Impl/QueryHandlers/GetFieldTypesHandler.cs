using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Queries;

namespace Montr.Metadata.Impl.QueryHandlers
{
	public class GetFieldTypesHandler : IRequestHandler<GetFieldTypes, IList<FieldType>>
	{
		public async Task<IList<FieldType>> Handle(GetFieldTypes request, CancellationToken cancellationToken)
		{
			var result = DataFieldTypes.Map.Keys.OrderBy(x => x).Select(x => new FieldType { Code = x, Name = x }).ToList();

			return await Task.FromResult(result);
		}
	}
}
