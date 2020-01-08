using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IFieldDataRepository : IRepository<FieldData>
	{
		Task<ApiResult> Validate(ManageFieldDataRequest request, CancellationToken cancellationToken);

		Task<ApiResult> Insert(ManageFieldDataRequest request, CancellationToken cancellationToken);

		Task<ApiResult> Update(ManageFieldDataRequest request, CancellationToken cancellationToken);

		Task<ApiResult> Delete(DeleteFieldDataRequest request, CancellationToken cancellationToken);
	}
}
