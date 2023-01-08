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
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Settings.Entities;
using Montr.Settings.Events;

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

		public IUpdatableSettings GetSettings(string entityTypeCode, Guid entityUid, Type ofSettings)
		{
			return new UpdatableSettings(ofSettings, this, entityTypeCode, entityUid);
		}

		public IUpdatableSettings<TSettings> GetSettings<TSettings>(string entityTypeCode, Guid entityUid)
		{
			return new UpdatableSettings<TSettings>(this, entityTypeCode, entityUid);
		}

		// todo: change first parameter to ICollection<(string, string)> and convert outside of method
		public async Task<ApiResult> Update(string entityTypeCode, Guid entityUid,
			ICollection<(string, object)> values, CancellationToken cancellationToken)
		{
			var affected = 0;

			var utcNow = _dateTimeProvider.GetUtcNow();

			using (var db = _dbContextFactory.Create())
			{
				foreach (var (key, value) in values)
				{
					var stringValue = value != null ? Convert.ToString(value, CultureInfo.InvariantCulture) : null;

					var updated = await db.GetTable<DbSettings>()
						.Where(x => x.EntityTypeCode == entityTypeCode &&
									x.EntityUid == entityUid &&
						            x.Key == key)
						.Set(x => x.Value, stringValue)
						.Set(x => x.ModifiedAtUtc, utcNow)
						.UpdateAsync(cancellationToken);

					affected += updated;

					if (updated == 0)
					{
						var inserted = await db.GetTable<DbSettings>()
							.Value(x => x.EntityTypeCode, entityTypeCode)
							.Value(x => x.EntityUid, entityUid)
							.Value(x => x.Key, key)
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
				await _mediator.Publish(new SettingsChanged
				{
					EntityTypeCode = entityTypeCode,
					EntityUid = entityUid,
					Values = values
				}, cancellationToken);
			};

			return new ApiResult { AffectedRows = affected };
		}

		private class UpdatableSettings : IUpdatableSettings
		{
			private readonly Type _ofSettings;
			private readonly ISettingsRepository _repository;
			private readonly string _entityTypeCode;
			private readonly Guid _entityUid;

			private readonly ICollection<(string, object)> _values = new List<(string, object)>();

			public UpdatableSettings(Type ofSettings,
				ISettingsRepository repository, string entityTypeCode, Guid entityUid)
			{
				_ofSettings = ofSettings;
				_repository = repository;
				_entityTypeCode = entityTypeCode;
				_entityUid = entityUid;
			}

			public IUpdatableSettings Set(string key, object value)
			{
				var fullKey = _ofSettings.FullName + ":" + key;

				_values.Add((fullKey, value));

				return this;
			}

			public async Task<ApiResult> Update(CancellationToken cancellationToken)
			{
				return await _repository.Update(_entityTypeCode, _entityUid, _values, cancellationToken);
			}
		}

		private class UpdatableSettings<TSettings> : UpdatableSettings, IUpdatableSettings<TSettings>
		{
			public UpdatableSettings(ISettingsRepository repository, string entityTypeCode, Guid entityUid)
				: base(typeof(TSettings), repository, entityTypeCode, entityUid)
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
