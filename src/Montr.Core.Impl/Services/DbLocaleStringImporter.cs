using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Montr.Core.Impl.Entities;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	public class DbLocaleStringImporter : ILocaleStringImporter
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbLocaleStringImporter(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task Import(IList<LocaleStringList> list, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				foreach (var locale in list)
				{
					foreach (var module in locale.Modules)
					{
						var deleted = await db.GetTable<DbLocaleString>()
							.Where(x => x.Locale == locale.Locale && x.Module == module.Module)
							.DeleteAsync(cancellationToken);

						var inserted = module.Items.Select(x => new DbLocaleString
						{
							Locale = locale.Locale,
							Module = module.Module,
							Key = x.Key,
							Value = x.Value
						});

						var copied = db.BulkCopy(inserted);

						// todo: write audit logs
					}
				}
			}
		}
	}
}
