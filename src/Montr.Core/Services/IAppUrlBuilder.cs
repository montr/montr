using System.Collections.Generic;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Montr.Core.Services
{
	public interface IAppUrlBuilder
	{
		string Build(string path, IDictionary<string, string> queryString = null);
	}

	public class DefaultAppUrlBuilder : IAppUrlBuilder
	{
		private readonly IConfiguration _configuration;

		public DefaultAppUrlBuilder(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string Build(string path, IDictionary<string, string> queryString = null)
		{
			var options = _configuration.GetSection("App").Get<AppOptions>();

			var result = $"{options.AppUrl}{path}";

			if (queryString != null)
			{
				result = QueryHelpers.AddQueryString(result, queryString);
			}

			return result;
		}
	}
}
