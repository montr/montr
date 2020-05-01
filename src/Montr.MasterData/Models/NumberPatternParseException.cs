using System;
using System.Runtime.Serialization;

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

		protected NumberPatternParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public int Position { get; set; }

		public TokenType TokenType { get; set; }
	}
}
