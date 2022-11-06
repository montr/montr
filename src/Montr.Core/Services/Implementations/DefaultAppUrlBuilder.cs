using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace Montr.Core.Services.Implementations
{
	public class DefaultAppUrlBuilder : IAppUrlBuilder
	{
		private readonly IOptionsMonitor<AppOptions> _optionsAccessor;

		public DefaultAppUrlBuilder(IOptionsMonitor<AppOptions> optionsAccessor)
		{
			_optionsAccessor = optionsAccessor;
		}

		public string Build(string path, IDictionary<string, string> queryString = null)
		{
			var options = _optionsAccessor.CurrentValue;

			if (options.AppUrl == null) throw new InvalidOperationException("AppOptions is not configured properly, AppUrl is not specified.");

			var result = $"{options.AppUrl}{path}";

			if (queryString != null)
			{
				result = QueryHelpers.AddQueryString(result, queryString);
			}

			return result;
		}
	}
}
