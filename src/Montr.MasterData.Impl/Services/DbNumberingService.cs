using System;
using System.Linq;
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

		public DbNumberingService(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<string> GenerateNumber(Guid numeratorUid)
		{
			using (var db = _dbContextFactory.Create())
			{
				var dbNumerator = await db
					.GetTable<DbNumerator>()
					.Where(x => x.Uid == numeratorUid)
					.FirstAsync();

				var dbNumeratorCounters = await db
					.GetTable<DbNumeratorCounter>()
					.Where(x => x.NumeratorUid == numeratorUid)
					.ToListAsync();

			}

			return string.Empty;
		}
	}
}
