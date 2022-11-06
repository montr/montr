using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Settings.Services
{
	public interface ISettingsRepository
	{
		IUpdatableSettings<TSettings> GetSettings<TSettings>();

		Task<int> Update<TSettings>(ICollection<(string, object)> values, CancellationToken cancellationToken);
	}

	public interface IUpdatableSettings<TSettings>
	{
		IUpdatableSettings<TSettings> Set<TValue>(Expression<Func<TSettings, TValue>> key, TValue value);

		Task<int> Update(CancellationToken cancellationToken);
	}
}
