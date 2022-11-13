using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	/// <summary>
	/// Task executed after all modules initialized and before application start.
	/// </summary>
	public interface IStartupTask
	{
		Task Run(CancellationToken cancellationToken);
	}

	/// <summary>
	/// Task executed after all modules initialized, all IStartupTask and before application start.
	/// IPostStartupTask executed in reverse registration order
	/// </summary>
	public interface IPostStartupTask
	{
		Task Run(CancellationToken cancellationToken);
	}
}
