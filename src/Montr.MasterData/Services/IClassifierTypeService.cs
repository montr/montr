using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	// todo: merge with IRepository<ClassifierType>?
	public interface IClassifierTypeService
	{
		Task<ClassifierType> TryGet(string typeCode, CancellationToken cancellationToken);

		Task<ClassifierType> GetClassifierType(string typeCode, CancellationToken cancellationToken);

		Task<ApiResult> Insert(ClassifierType item, CancellationToken cancellationToken);
	}
}
