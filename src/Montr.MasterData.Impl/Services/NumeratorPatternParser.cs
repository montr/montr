using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Montr.MasterData.Impl.Services
{
	public class NumeratorPatternParser
	{
		private static readonly Regex Rx = new Regex(@"\{(.*?)\}", RegexOptions.Compiled);

		public Result Parse(string pattern)
		{
			if (pattern == null) throw new ArgumentNullException(nameof(pattern));

			var matches = Rx.Matches(pattern);

			return new Result
			{
				Success = true,
				Tags = matches.Select(x => x.Value).ToArray()
			};
		}

		public class Result
		{
			public bool Success { get; set; }

			public string[] Tags { get; set; }
		}
	}
}
