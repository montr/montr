using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace Montr.Core.Services
{
	public interface ICache
	{
		Task<T> GetOrCreate<T>(string key, Func<Task<T>> valueFactory, CancellationToken token) where T : class;
	}

	public class CombinedCache : ICache
	{
		private readonly IMemoryCache _memoryCache;
		private readonly IDistributedCache _distributedCache;
		private readonly IBinarySerializer _serializer;

		public CombinedCache(IMemoryCache memoryCache, IDistributedCache distributedCache, IBinarySerializer serializer)
		{
			_memoryCache = memoryCache;
			_distributedCache = distributedCache;
			_serializer = serializer;
		}

		public async Task<T> GetOrCreate<T>(string key, Func<Task<T>> valueFactory, CancellationToken token) where T : class
		{
			return await _memoryCache.GetOrCreateAsync(key, async entry =>  
			{
				var result = await _distributedCache.GetAsync(key, token);

				if (result == null)
				{
					// todo: pass options as params
					var options = new DistributedCacheEntryOptions();

					var value = await valueFactory();

					await _distributedCache.SetAsync(key, _serializer.Serialize(value), options, token);

					return value;
				}

				return _serializer.Deserialize<T>(result);
			});
		}
	}
}
