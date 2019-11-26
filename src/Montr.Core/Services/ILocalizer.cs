using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface ILocalizer
	{
		string Get<T>(Expression<Func<T, string>> key);

		string Get(string key);
	}

	public class DefaultLocalizer : ILocalizer
	{
		private readonly IRepository<LocaleString> _repository;

		public DefaultLocalizer(IRepository<LocaleString> repository)
		{
			_repository = repository;
		}

		public string Get<T>(Expression<Func<T, string>> key)
		{
			return Get(ExpressionHelper.GetFullName(key));
		}

		public string Get(string key)
		{
			// todo: add caching
			// todo: async
			var result = _repository.Search(new LocaleStringSearchRequest
			{
				Locale = Thread.CurrentThread.CurrentCulture.Name,
				PageSize = 0 // disable paging
			}, CancellationToken.None);

			var dict = result.Result.Rows.ToDictionary(x => x.Key);

			return dict.GetValueOrDefault(key)?.Value ?? key;
		}
	}
}
