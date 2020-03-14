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

		public async Task<string> GenerateNumber(Guid numeratorUid, string entityTypeCode, Guid enityUid, CancellationToken cancellationToken)
		{
			// todo: add distributed lock

			using (var db = _dbContextFactory.Create())
			{
				var dbNumerator = await db
					.GetTable<DbNumerator>()
					.Where(x => x.Uid == numeratorUid)
					.FirstAsync(cancellationToken);

				var dbNumeratorCounters = await db
					.GetTable<DbNumeratorCounter>()
					.Where(x => x.NumeratorUid == numeratorUid)
					.ToListAsync(cancellationToken);

				var key = string.Empty;

				_patternParser.Parse(dbNumerator.Pattern);

				long value;

				var dbNumeratorCounter = dbNumeratorCounters.Find(x => x.Key == key);

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
