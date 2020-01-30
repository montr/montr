using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierTypeService
	{
		Task<ClassifierType> GetClassifierType(string typeCode, CancellationToken cancellationToken);
	}
}
