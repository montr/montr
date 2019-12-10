using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface ILocalizer
	{
		Task<string> Get<T>(Expression<Func<T, string>> key, CancellationToken cancellationToken);

		Task<string> Get(string key, CancellationToken cancellationToken);
	}

	public class DefaultLocalizer : ILocalizer
	{
		private readonly IRepository<LocaleString> _repository;
		private readonly ICache _cache;

		public DefaultLocalizer(
			IRepository<LocaleString> repository,
			ICache cache)
		{
			_repository = repository;
			_cache = cache;
		}

		public async Task<string> Get<T>(Expression<Func<T, string>> keyExpr, CancellationToken cancellationToken)
		{
			// todo: remove module name from key?
			var key = ExpressionHelper.GetFullName(keyExpr);

			return await Get(key, cancellationToken);
		}

		public async Task<string> Get(string key, CancellationToken cancellationToken)
		{
			// todo: use fallback lang
			var lang = Thread.CurrentThread.CurrentCulture.Name;

			// todo: add module key to cache key
			var cacheKey = $"{typeof(DefaultLocalizer).FullName}_{lang}";

			var resources = await _cache.GetOrCreate(cacheKey, async () =>
			{
				var request = new LocaleStringSearchRequest
				{
					Locale = lang,
					PageSize = 0 // disable paging
				};

				var result = await _repository.Search(request, cancellationToken);

				return result.Rows.ToDictionary(x => x.Key);
			}, cancellationToken);

			return resources.GetValueOrDefault(key)?.Value /*?? key*/;
		}
	}
}
