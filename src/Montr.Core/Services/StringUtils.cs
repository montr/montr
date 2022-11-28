using System;
using System.Collections.Generic;

namespace Montr.Core.Services
{
	public static class StringUtils
	{
		public static IEnumerable<string> SplitUpperCase(string source)
		{
			if (source == null) return Array.Empty<string>();

			if (source.Length == 0) return new[] { string.Empty };

			var words = new List<string>();
			var wordStartIndex = 0;

			var letters = source.ToCharArray();
			var previousChar = char.MinValue;

			for (var i = 1; i < letters.Length; i++)
			{
				if (char.IsUpper(letters[i]) && char.IsWhiteSpace(previousChar) == false)
				{
					words.Add(new string(letters, wordStartIndex, i - wordStartIndex));
					wordStartIndex = i;
				}

				previousChar = letters[i];
			}

			words.Add(new string(letters, wordStartIndex, letters.Length - wordStartIndex));

			return words;
		}
	}
}
