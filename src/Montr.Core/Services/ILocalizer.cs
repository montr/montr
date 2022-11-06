using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	public interface ILocalizer
	{
		Task<string> Get<T>(Expression<Func<T, string>> key, CancellationToken cancellationToken);

		Task<string> Get(string key, CancellationToken cancellationToken);
	}
}
