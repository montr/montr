using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;

namespace Montr.MasterData.Impl.Services
{
	public class ClosureTableHandler
	{
		private readonly DbContext _db;

		public ClosureTableHandler(DbContext db)
		{
			_db = db;
		}

		public async Task<long> Insert(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
		{
			// insert self closure with level 0
			await _db.GetTable<DbClassifierClosure>()
				.Value(x => x.ParentUid, itemUid)
				.Value(x => x.ChildUid, itemUid)
				.Value(x => x.Level, 0)
				.InsertAsync(cancellationToken);

			if (parentUid.HasValue)
			{
				// insert parent closures with level + 1
				var closures = await _db.GetTable<DbClassifierClosure>()
					.Where(x => x.ChildUid == parentUid.Value)
					.Select(x => new DbClassifierClosure
					{
						ParentUid = x.ParentUid,
						ChildUid = itemUid,
						Level = (short)(x.Level + 1)
					})
					.ToListAsync(cancellationToken);

				return _db.BulkCopy(closures).RowsCopied + 1;
			}

			return 1;
		}
	}
}
