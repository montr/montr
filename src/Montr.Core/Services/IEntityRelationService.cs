using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IEntityRelationService
	{
		Task<IList<EntityRelation>> List(EntityRelationSearchRequest searchRequest, CancellationToken cancellationToken = default);

		Task<ApiResult> Insert(EntityRelation relation, CancellationToken cancellationToken = default);

		Task<ApiResult> Delete(EntityRelation relation, CancellationToken cancellationToken = default);
	}
}
