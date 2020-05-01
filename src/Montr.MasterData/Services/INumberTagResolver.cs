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

		Task<NumberTagResolveResult> Resolve(GenerateNumberRequest request, IEnumerable<string> tags, CancellationToken cancellationToken);
	}

	public class NumberTagResolveResult
	{
		public DateTime? Date { get; set; }

		public IDictionary<string, string> Values { get; set; }
	}
}
