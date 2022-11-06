using System;

namespace Montr.Core.Services.Impl
{
	public class DefaultDateTimeProvider : IDateTimeProvider
	{
		public DateTime GetUtcNow()
		{
			return DateTime.UtcNow;
		}
	}
}