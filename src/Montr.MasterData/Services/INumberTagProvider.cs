using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.MasterData.Services
{
	public interface INumberTagProvider
	{
		bool Supports(string entityTypeCode, out string[] supportedTags);

		Task Resolve(string entityTypeCode, Guid enityUid, out DateTime? date,
			IEnumerable<string> tags, IDictionary<string, string> values, CancellationToken cancellationToken);
	}
}
