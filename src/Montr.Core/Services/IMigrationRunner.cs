using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	public interface IMigrationRunner
	{
		Task Run(CancellationToken cancellationToken);
	}
}
