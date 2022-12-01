using System;

namespace Montr.Core.Services
{
	public static class OptionsUtils
	{
		public static string GetOptionsSectionKey<TOptions>()
		{
			return GetOptionsSectionKey(typeof(TOptions));
		}

		public static string GetOptionsSectionKey(Type ofOptions)
		{
			return ofOptions.FullName;
		}
	}
}
