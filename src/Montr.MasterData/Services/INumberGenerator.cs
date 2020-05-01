using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface INumberGenerator
	{
		Task<string> GenerateNumber(GenerateNumberRequest request, CancellationToken cancellationToken);
	}
}
