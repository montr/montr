using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	public interface IOptionsRepository
	{
		IUpdatableOptions<TOptions> GetOptions<TOptions>();

		Task<int> Update<TOptions>(IEnumerable<(string, object)> values, CancellationToken cancellationToken);
	}

	public interface IUpdatableOptions<TOptions>
	{
		IUpdatableOptions<TOptions> Set<TValue>(Expression<Func<TOptions, TValue>> key, TValue value);

		Task<int> Update(CancellationToken cancellationToken);
	}
}
