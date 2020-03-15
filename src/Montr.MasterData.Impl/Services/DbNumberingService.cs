using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbNumberingService : INumberingService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly NumeratorPatternParser _patternParser;

		public DbNumberingService(IDbContextFactory dbContextFactory, NumeratorPatternParser patternParser)
		{
			_dbContextFactory = dbContextFactory;
			_patternParser = patternParser;
		}

		public async Task<string> GenerateNumber(string entityTypeCode, Guid enityUid, CancellationToken cancellationToken)
		{
			// todo: add distributed lock
			using (var db = _dbContextFactory.Create())
			{
				var dbNumeratorEntity = await db.GetTable<DbNumeratorEntity>()
					.Where(x => x.EntityUid == enityUid)
					.FirstOrDefaultAsync(cancellationToken);

				if (dbNumeratorEntity == null) return null;

				var numeratorUid = dbNumeratorEntity.NumeratorUid;

				var dbNumerator = await db
					.GetTable<DbNumerator>()
					.Where(x => x.Uid == numeratorUid)
					.FirstAsync(cancellationToken);

				// todo: generate key
				_patternParser.Parse(dbNumerator.Pattern);
				var key = string.Empty;

				var dbNumeratorCounter = await db
					.GetTable<DbNumeratorCounter>()
					.Where(x => x.NumeratorUid == numeratorUid && x.Key == key)
					.FirstOrDefaultAsync(cancellationToken);

				long value;

				if (dbNumeratorCounter == null)
				{
					value = 1;

					await db
						.GetTable<DbNumeratorCounter>()
						.Value(x => x.NumeratorUid, numeratorUid)
						.Value(x => x.Key, key)
						.Value(x => x.Value, value)
						.InsertAsync(cancellationToken);
				}
				else
				{
					value = dbNumeratorCounter.Value + 1;

					await db
						.GetTable<DbNumeratorCounter>()
						.Where(x => x.NumeratorUid == numeratorUid && x.Key == key)
						.Set(x => x.Value, value)
						.UpdateAsync(cancellationToken);
				}

				return dbNumerator.Pattern.Replace("{Number}", value.ToString("D5"));
			}
		}
	}
}
