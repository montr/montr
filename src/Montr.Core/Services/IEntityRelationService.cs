using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services;

public interface IEntityRelationService
{
	Task<ApiResult> Insert(EntityRelation relation, CancellationToken cancellationToken);

	Task<ApiResult> Delete(EntityRelation relation, CancellationToken cancellationToken);
}
