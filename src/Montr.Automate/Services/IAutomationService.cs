using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Commands;
using Montr.Core.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationService
	{
		Task<ApiResult> Insert(InsertAutomation request, CancellationToken cancellationToken);

		Task<ApiResult> Update(UpdateAutomation request, CancellationToken cancellationToken);

		Task<ApiResult> Delete(DeleteAutomation request, CancellationToken cancellationToken);
	}
}
