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

		public async Task Delete(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
		{
			// reset parent of groups
			await _db.GetTable<DbClassifierGroup>()
				.Where(x => x.ParentUid == itemUid)
				.Set(x => x.ParentUid, parentUid)
				.UpdateAsync(cancellationToken);

			// reset parent of items
			if (parentUid != null)
			{
				await _db.GetTable<DbClassifierLink>()
					.Where(x => x.GroupUid == itemUid)
					.Set(x => x.GroupUid, parentUid)
					.UpdateAsync(cancellationToken);
			}
			else
			{
				await _db.GetTable<DbClassifierLink>()
					.Where(x => x.GroupUid == itemUid)
					.DeleteAsync(cancellationToken);
			}

			// move children to parent with level - 1
			// (if no parent - all children stay the same)
			if (parentUid != null)
			{
				await (from parent in _db.GetTable<DbClassifierClosure>()
						where parent.ChildUid == itemUid && parent.Level > 0
						from child in _db.GetTable<DbClassifierClosure>()
						where child.ParentUid == itemUid && child.Level > 0
						join updatable in _db.GetTable<DbClassifierClosure>()
							on new { parent.ParentUid, child.ChildUid }
							equals new { updatable.ParentUid, updatable.ChildUid }
						select updatable)
					.AsUpdatable()
					.Set(x => x.Level, x => x.Level - 1)
					.UpdateAsync(cancellationToken);
			}

			// delete unused closure
			await _db.GetTable<DbClassifierClosure>()
				.Where(x => x.ParentUid == itemUid)
				.DeleteAsync(cancellationToken);
		}
	}
}
