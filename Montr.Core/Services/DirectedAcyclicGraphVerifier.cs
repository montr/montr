using System;
using System.Collections.Generic;
using System.Linq;

namespace Montr.Core.Services
{
	public class DirectedAcyclicGraphVerifier
	{
		/// <summary>
		/// Topological sort.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <param name="source"></param>
		/// <param name="getKey"></param>
		/// <param name="getDependencies"></param>
		/// <returns></returns>
		public static IList<T> Sort<T, TKey>(ICollection<T> source, Func<T, TKey> getKey, Func<T, IEnumerable<TKey>> getDependencies)
		{
			return Sort(source, RemapDependencies(source, getKey, getDependencies));
		}

		/*public static IList<T> Sort<T, TKey>(IEnumerable<T> source, Func<T, TKey> getKey, Func<T, IEnumerable<T>> getDependencies)
		{
			return Sort(source, getDependencies, new GenericEqualityComparer<T, TKey>(getKey));
		}*/

		public static IList<T> Sort<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies, IEqualityComparer<T> comparer = null)
		{
			var sorted = new List<T>();
			var visited = new Dictionary<T, bool>(comparer);

			foreach (var item in source)
			{
				Visit(item, getDependencies, sorted, visited);
			}

			return sorted;
		}

		private static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, ICollection<T> sorted, IDictionary<T, bool> visited)
		{
			var alreadyVisited = visited.TryGetValue(item, out var inProcess);

			if (alreadyVisited)
			{
				if (inProcess)
				{
					throw new InvalidOperationException("Cyclic dependency found.");
				}
			}
			else
			{
				visited[item] = true;

				var dependencies = getDependencies(item);
				if (dependencies != null)
				{
					foreach (var dependency in dependencies)
					{
						Visit(dependency, getDependencies, sorted, visited);
					}
				}

				visited[item] = false;
				sorted.Add(item);
			}
		}

		private static Func<T, IEnumerable<T>> RemapDependencies<T, TKey>(
			IEnumerable<T> source, Func<T, TKey> getKey, Func<T, IEnumerable<TKey>> getDependencies)
		{
			var map = source.ToDictionary(getKey);

			return item =>
			{
				var dependencies = getDependencies(item);

				return dependencies?.Select(key => map[key]);
			};
		}

		public class GenericEqualityComparer<TItem, TKey> : EqualityComparer<TItem>
		{
			private readonly Func<TItem, TKey> _getKey;
			private readonly EqualityComparer<TKey> _keyComparer;

			public GenericEqualityComparer(Func<TItem, TKey> getKey)
			{
				_getKey = getKey;
				_keyComparer = EqualityComparer<TKey>.Default;
			}

			public override bool Equals(TItem x, TItem y)
			{
				if (x == null && y == null)
				{
					return true;
				}

				if (x == null || y == null)
				{
					return false;
				}

				return _keyComparer.Equals(_getKey(x), _getKey(y));
			}

			public override int GetHashCode(TItem obj)
			{
				if (obj == null)
				{
					return 0;
				}

				return _keyComparer.GetHashCode(_getKey(obj));
			}
		}
	}
}
