using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IFieldDataRepository : IRepository<FieldData>
	{
		Task<ApiResult> Insert(FieldDataRequest request, CancellationToken cancellationToken);

		Task<ApiResult> Update(FieldDataRequest request, CancellationToken cancellationToken);

		Task<ApiResult> Delete(DeleteFieldDataRequest request, CancellationToken cancellationToken);
	}
}
