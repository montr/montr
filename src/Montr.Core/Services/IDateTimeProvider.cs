using System;

namespace Montr.Core.Services
{
	public interface IDateTimeProvider
	{
		DateTime GetUtcNow();
	}

	public class DefaultDateTimeProvider : IDateTimeProvider
	{
		public DateTime GetUtcNow()
		{
			return DateTime.UtcNow;
		}
	}
}
