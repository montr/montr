using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Settings.Services
{
	public interface ISettingsRepository
	{
		IUpdatableSettings GetSettings(Type ofSettings);

		IUpdatableSettings<TSettings> GetSettings<TSettings>();

		Task<int> Update(ICollection<(string, object)> values, CancellationToken cancellationToken);
	}

	public interface IUpdatableSettings
	{
		IUpdatableSettings Set(string key, object value);

		Task<int> Update(CancellationToken cancellationToken);
	}

	public interface IUpdatableSettings<TSettings> : IUpdatableSettings
	{
		IUpdatableSettings<TSettings> Set<TValue>(Expression<Func<TSettings, TValue>> key, TValue value);
	}
}
