using System;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	public interface ICache
	{
		Task<T> GetOrCreate<T>(string key, Func<Task<T>> valueFactory, CancellationToken token) where T : class;
	}
}
