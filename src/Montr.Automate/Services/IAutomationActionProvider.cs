using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationActionProvider
	{
		Task Execute(AutomationAction automationAction, object entity, CancellationToken cancellationToken);
	}
}
