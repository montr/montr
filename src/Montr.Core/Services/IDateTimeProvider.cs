using System;

namespace Montr.Core.Services
{
	public interface IDateTimeProvider
	{
		DateTime GetUtcNow();
	}
}
