using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;

namespace Montr.Automate.Services
{
	public interface IRecipientResolver
	{
		Task<Recipient> Resolve(string recipient, AutomationContext automationContext, CancellationToken cancellationToken);
	}
}
