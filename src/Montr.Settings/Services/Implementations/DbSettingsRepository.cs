using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using LinqToDB;
using MediatR;
using Montr.Core.Events;
using Montr.Core.Services;
using Montr.Settings.Entities;

namespace Montr.Settings.Services.Implementations
{
	public class DbSettingsRepository : ISettingsRepository
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IPublisher _mediator;

		public DbSettingsRepository(IDbContextFactory dbContextFactory, IDateTimeProvider dateTimeProvider, IPublisher mediator)
		{
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_mediator = mediator;
		}

		public IUpdatableSettings GetSettings(Type ofSettings)
		{
			return new UpdatableSettings(ofSettings, this);
		}

		public IUpdatableSettings<TSettings> GetSettings<TSettings>()
		{
			return new UpdatableSettings<TSettings>(this);
		}

		// todo: change first parameter to ICollection<(string, string)> and convert outside of method
		public async Task<int> Update(ICollection<(string, object)> values, CancellationToken cancellationToken)
		{
			var affected = 0;

			var utcNow = _dateTimeProvider.GetUtcNow();

			using (var db = _dbContextFactory.Create())
			{
				foreach (var (key, value) in values)
				{
					var stringValue = value != null ? Convert.ToString(value, CultureInfo.InvariantCulture) : null;

					var updated = await db.GetTable<DbSettings>()
						.Where(x => x.Id == key)
						.Set(x => x.Value, stringValue)
						.Set(x => x.ModifiedAtUtc, utcNow)
						.UpdateAsync(cancellationToken);

					affected += updated;

					if (updated == 0)
					{
						var inserted = await db.GetTable<DbSettings>()
							.Value(x => x.Id, key)
							.Value(x => x.Value, stringValue)
							.Value(x => x.CreatedAtUtc, utcNow)
							.InsertAsync(cancellationToken);

						affected += inserted;
					}
				}
			}

			// todo: move to UnitOfWork.Completed
			// ReSharper disable once PossibleNullReferenceException
			Transaction.Current.TransactionCompleted += async (sender, args) =>
			{
				await _mediator.Publish(new SettingsChanged { Values = values }, cancellationToken);
			};

			return affected;
		}

		private class UpdatableSettings : IUpdatableSettings
		{
			private readonly Type _ofSettings;
			private readonly ISettingsRepository _repository;

			private readonly ICollection<(string, object)> _values = new List<(string, object)>();

			public UpdatableSettings(Type ofSettings, ISettingsRepository repository)
			{
				_ofSettings = ofSettings;
				_repository = repository;
			}

			public IUpdatableSettings Set(string key, object value)
			{
				var fullKey = _ofSettings.FullName + ":" + key;

				_values.Add((fullKey, value));

				return this;
			}

			public async Task<int> Update(CancellationToken cancellationToken)
			{
				return await _repository.Update(_values, cancellationToken);
			}
		}

		private class UpdatableSettings<TSettings> : UpdatableSettings, IUpdatableSettings<TSettings>
		{
			public UpdatableSettings(ISettingsRepository repository) : base(typeof(TSettings), repository)
			{
			}

			public IUpdatableSettings<TSettings> Set<TValue>(Expression<Func<TSettings, TValue>> keyExpr, TValue value)
			{
				base.Set(ExpressionHelper.GetMemberName(keyExpr), value);

				return this;
			}
		}
	}
}
