using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierRegistrator
	{
		Task<ApiResult> Register(Classifier item, CancellationToken cancellationToken);
	}
}
