using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Docs.Commands;

namespace Montr.Docs.Services
{
	public interface IProcessService
	{
		Task<ApiResult> Insert(InsertProcessStep request, CancellationToken cancellationToken);

		Task<ApiResult> Update(UpdateProcessStep request, CancellationToken cancellationToken);

		Task<ApiResult> Delete(DeleteProcessStep request, CancellationToken cancellationToken);
	}
}