using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;

namespace Montr.Automate.Services
{
	// todo: move separate project and remove reference to Messages?
	public interface IRecipientResolver
	{
		Task<Recipient> Resolve(string recipient, AutomationContext context, CancellationToken cancellationToken);
	}
}
