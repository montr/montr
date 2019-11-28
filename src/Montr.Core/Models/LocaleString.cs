using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Montr.Core.Models
{
	[Serializable]
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class LocaleString
	{
		private string DebuggerDisplay => $"{Key}: {Value}";

		public string Locale { get; set; }

		public string Module { get; set; }

		public string Key { get; set; }

		public string Value { get; set; }
	}

	public class LocaleStringList
	{
		public string Locale { get; set; }

		public IList<LocaleModuleStringList> Modules { get; set; }
	}

	public class LocaleModuleStringList
	{
		public string Module { get; set; }

		public IList<LocaleString> Items { get; set; }
	}
}
