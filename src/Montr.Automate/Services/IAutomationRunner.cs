using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationRunner
	{
		Task Run(AutomationContext context, CancellationToken cancellationToken);
	}
}
