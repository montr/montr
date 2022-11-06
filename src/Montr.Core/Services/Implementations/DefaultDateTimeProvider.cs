using System;

namespace Montr.Core.Services.Implementations
{
	public class DefaultDateTimeProvider : IDateTimeProvider
	{
		public DateTime GetUtcNow()
		{
			return DateTime.UtcNow;
		}
	}
}
