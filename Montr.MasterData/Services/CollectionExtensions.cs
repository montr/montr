using System.Collections.Generic;

namespace Montr.MasterData.Services
{
	public static class CollectionExtensions
	{
		public static bool IsNullOrEmpty<T>(this ICollection<T> source)
		{
			return source == null || source.Count == 0;
		}
	}
}
