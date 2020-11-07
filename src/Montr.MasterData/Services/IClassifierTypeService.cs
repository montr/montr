using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierTypeService
	{
		Task<ClassifierType> TryGet(string typeCode, CancellationToken cancellationToken);

		Task<ClassifierType> Get(string typeCode, CancellationToken cancellationToken);

		Task<ApiResult> Insert(ClassifierType item, CancellationToken cancellationToken);
	}
}
