using System;

namespace Montr.MasterData.Models
{
	public class NumberPatternParseException : Exception
	{
		public NumberPatternParseException()
		{
		}

		public NumberPatternParseException(string message) : base(message)
		{
		}

		public NumberPatternParseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public int Position { get; set; }

		public TokenType TokenType { get; set; }
	}
}
