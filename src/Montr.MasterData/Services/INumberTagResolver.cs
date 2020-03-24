using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface INumberTagResolver
	{
		bool Supports(GenerateNumberRequest request, out string[] supportedTags);

		Task Resolve(GenerateNumberRequest request, out DateTime? date,
			IEnumerable<string> tags, IDictionary<string, string> values, CancellationToken cancellationToken);
	}
}
