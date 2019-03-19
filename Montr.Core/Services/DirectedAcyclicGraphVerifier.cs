using System;
using System.Collections.Generic;
using System.Linq;

namespace Montr.Core.Services
{
	public class DirectedAcyclicGraphVerifier
	{
		public static IList<T> TopologicalSort<T, TKey>(ICollection<T> source,
			Func<T, TKey> getKey, Func<T, IEnumerable<TKey>> getDependencies)
		{
			var remapDependencies = RemapDependencies(source, getKey, getDependencies);

			var sorted = new List<T>();
			var visited = new Dictionary<T, bool>();

			foreach (var item in source)
			{
				Visit(item, remapDependencies, getKey, sorted, visited, getKey(item).ToString());
			}

			return sorted;
		}

		private static void Visit<T, TKey>(T item,
			Func<T, IEnumerable<T>> getDependencies, Func<T, TKey> getKey,
			ICollection<T> sorted, IDictionary<T, bool> visited, string path = null)
		{
			if (visited.TryGetValue(item, out var inProcess))
			{
				if (inProcess)
				{
					throw new InvalidOperationException("Cyclic dependency found: " + path);
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
						Visit(dependency, getDependencies, getKey, sorted, visited, path + " -> " + getKey(dependency));
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
				return getDependencies(item)?.Select(key => map[key]);
			};
		}
	}
}
