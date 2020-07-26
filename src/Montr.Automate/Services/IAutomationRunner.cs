using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Core.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationRunner
	{
		Task<ApiResult> Run(AutomationContext context, CancellationToken cancellationToken);
	}
}
