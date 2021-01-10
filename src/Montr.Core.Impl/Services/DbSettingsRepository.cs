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
using Montr.Core.Impl.Entities;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	public class DbSettingsRepository : ISettingsRepository
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IPublisher _mediator;

		public DbSettingsRepository(IDbContextFactory dbContextFactory, IPublisher mediator)
		{
			_dbContextFactory = dbContextFactory;
			_mediator = mediator;
		}

		public IUpdatableSettings<TSettings> GetSettings<TSettings>()
		{
			return new UpdatableSettings<TSettings>(this);
		}

		public async Task<int> Update<TSettings>(ICollection<(string, object)> values, CancellationToken cancellationToken)
		{
			var affected = 0;

			using (var db = _dbContextFactory.Create())
			{
				foreach (var (key, value) in values)
				{
					var stringValue = value != null ? Convert.ToString(value, CultureInfo.InvariantCulture) : null;

					var updated = await db.GetTable<DbSettings>()
						.Where(x => x.Id == key)
						.Set(x => x.Value, stringValue)
						.UpdateAsync(cancellationToken);

					affected += updated;

					if (updated == 0)
					{
						var inserted = await db.GetTable<DbSettings>()
							.Value(x => x.Id, key)
							.Value(x => x.Value, stringValue)
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

		private class UpdatableSettings<TSettings> : IUpdatableSettings<TSettings>
		{
			private readonly ISettingsRepository _repository;

			private readonly ICollection<(string, object)> _values = new List<(string, object)>();

			public UpdatableSettings(ISettingsRepository repository)
			{
				_repository = repository;
			}

			public IUpdatableSettings<TSettings> Set<TValue>(Expression<Func<TSettings, TValue>> keyExpr, TValue value)
			{
				var key = typeof(TSettings).FullName + ":" + ExpressionHelper.GetMemberName(keyExpr);

				_values.Add((key, value));

				return this;
			}

			public async Task<int> Update(CancellationToken cancellationToken)
			{
				return await _repository.Update<TSettings>(_values, cancellationToken);
			}
		}
	}
}
