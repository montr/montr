using System;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Numerator : Classifier
	{
		private string DebuggerDisplay => $"{Name} - {Pattern}";

		public static readonly StringComparer TagComparer = StringComparer.OrdinalIgnoreCase;

		public static readonly string DefaultPattern = "{Number}";

		public string EntityTypeCode { get; set; }

		public string Pattern { get; set; }

		// (?) tags used to build unique numerator keys (unique numbers in scope of these tags)
		// todo: display in UI as checkboxes (?) - only for documents (?)
		public string[] KeyTags { get; set; }

		public NumeratorPeriodicity Periodicity { get; set; }

		// todo: move to classifier
		// todo: add FK to classifier
		public bool IsActive { get; set; }

		public bool IsSystem { get; set; }
	}

	public enum NumeratorPeriodicity : byte
	{
		None,
		Day,
		// Week, // requires settings for start of week - Monday or Sunday
		Month,
		Quarter,
		Year
	}

	public abstract class Token
	{
	}

	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class TextToken : Token
	{
		private string DebuggerDisplay => $"Text: {Content}";

		public string Content { get; set; }
	}

	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class TagToken : Token
	{
		private string DebuggerDisplay => $"Tag: {{{Name}{(Args != null && Args.Length > 0 ? ':' + string.Join(':', Args) : string.Empty)}}}";

		public string Name { get; set; }

		public string[] Args { get; set; }
	}

	public enum TokenType : byte
	{
		Text,
		TagBegin,
		TagName,
		TagArgSeparator,
		TagArg,
		TagEnd,
		End
	}
}
