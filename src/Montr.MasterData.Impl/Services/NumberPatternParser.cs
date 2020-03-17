using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Montr.MasterData.Impl.Services
{
	public class NumberPatternParser
	{
		private static readonly Regex NumberPatternRegex = new Regex(@"\{(.*?)\}", RegexOptions.Compiled);

		public ISet<string> Parse(string pattern)
		{
			if (pattern == null) throw new ArgumentNullException(nameof(pattern));

			var matches = NumberPatternRegex.Matches(pattern);

			return matches.Select(x => x.Value).ToHashSet();
		}
	}
}
