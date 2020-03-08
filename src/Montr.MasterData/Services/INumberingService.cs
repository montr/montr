using System;
using System.Threading.Tasks;

namespace Montr.MasterData.Services
{
	public interface INumberingService
	{
		Task<string> GenerateNumber(Guid numeratorUid);
	}
}
