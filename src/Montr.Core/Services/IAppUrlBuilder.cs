using System.Collections.Generic;

namespace Montr.Core.Services
{
	public interface IAppUrlBuilder
	{
		string Build(string path, IDictionary<string, string> queryString = null);
	}
}
