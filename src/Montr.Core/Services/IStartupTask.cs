using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	/// <summary>
	/// Task executed before application start.
	/// </summary>
	public interface IStartupTask
	{
		Task Run(CancellationToken cancellationToken);
	}
}
