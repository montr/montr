using System;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.MasterData.Services
{
	public interface INumberGenerator
	{
		Task<string> GenerateNumber(string entityTypeCode, Guid enityUid, CancellationToken cancellationToken);
	}
}
