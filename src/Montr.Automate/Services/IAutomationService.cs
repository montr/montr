using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationService
	{
		Task OnChange(AutomationContext context, CancellationToken cancellationToken);
	}
}
