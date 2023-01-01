using System;
using System.Collections.Generic;
using System.Linq;
using Montr.Core.Services;

namespace Montr.Metadata.Services.Implementations
{
	public static class FieldNameUtils
	{
		private static readonly List<string> IgnoredWords = new() { "uid" };

		public static string BuildSettingsName(string source)
		{
			var words = StringUtils.SplitUpperCase(source).Where(NotIgnoredWord);

			return string.Join(" ", words);
		}

		private static bool NotIgnoredWord(string word)
		{
			return IgnoredWords.Contains(word, StringComparer.OrdinalIgnoreCase) == false;
		}
	}
}
