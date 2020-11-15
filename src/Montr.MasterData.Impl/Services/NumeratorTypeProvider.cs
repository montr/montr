using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class NumeratorTypeProvider : IClassifierTypeProvider
	{
		private readonly IDbContextFactory _dbContextFactory;
		public static readonly string TypeCode = nameof(Numerator);

		public NumeratorTypeProvider(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public Type ClassifierType { get; } = typeof(Numerator);

		public Task<Classifier> Create(ClassifierType type, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task Insert(ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			var numerator = (Numerator)item;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbNumerator>()
					.Value(x => x.Uid, item.Uid)
					.Value(x => x.EntityTypeCode, numerator.EntityTypeCode)
					// .Value(x => x.Name, numerator.Name)
					.Value(x => x.Pattern, numerator.Pattern)
					.Value(x => x.Periodicity, numerator.Periodicity.ToString())
					.Value(x => x.IsActive, numerator.IsActive)
					.Value(x => x.IsSystem, numerator.IsSystem)
					.InsertAsync(cancellationToken);
			}
		}

		public async Task Update(ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			var numerator = (Numerator)item;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbNumerator>()
					.Where(x => x.Uid == item.Uid)
					// .Set(x => x.Name, item.Name)
					.Set(x => x.Pattern, numerator.Pattern)
					.Set(x => x.Periodicity, numerator.Periodicity.ToString())
					.Set(x => x.IsActive, numerator.IsActive)
					// .Set(x => x.IsSystem, item.IsSystem)
					.UpdateAsync(cancellationToken);
			}
		}
	}
}
