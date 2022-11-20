using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Settings.Services
{
	public interface ISettingsRepository
	{
		IUpdatableSettings GetSettings(string entityTypeCode, Guid entityUid, Type ofSettings);

		IUpdatableSettings<TSettings> GetSettings<TSettings>(string entityTypeCode, Guid entityUid);

		Task<int> Update(string entityTypeCode, Guid entityUid, ICollection<(string, object)> values, CancellationToken cancellationToken);
	}

	public static class SettingsRepositoryExtensions
	{
		public static IUpdatableSettings<TSettings> GetApplicationSettings<TSettings>(this ISettingsRepository repository)
		{
			return repository.GetSettings<TSettings>(Application.EntityTypeCode, Application.EntityUid);
		}
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
