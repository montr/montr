using System;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.MasterData.Services
{
	public interface INumberingService
	{
		Task<string> GenerateNumber(string entityTypeCode, Guid enityUid, CancellationToken cancellationToken);
	}

	public interface INumeratorTagProvider
	{
		string[] GetAvailableTags();

		void ResolveValues(string entityTypeCode, Guid enityUid, string[] tags, CancellationToken cancellationToken);
	}
}
