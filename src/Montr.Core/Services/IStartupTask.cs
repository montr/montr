using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	/// <summary>
	/// Task executed before application start.
	/// todo: move all startup tasks to impl, like RegisterMessageTemplateStartupTask
	/// </summary>
	public interface IStartupTask
	{
		Task Run(CancellationToken cancellationToken);
	}
}
