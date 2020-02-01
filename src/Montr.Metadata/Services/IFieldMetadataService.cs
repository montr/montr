using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IFieldMetadataService
	{
		Task<ApiResult> Insert(ManageFieldMetadataRequest request, CancellationToken cancellationToken);
	}
}
