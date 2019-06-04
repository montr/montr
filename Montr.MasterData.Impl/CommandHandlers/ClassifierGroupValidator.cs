using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Impl.CommandHandlers
{
	// todo: add tests
	public class ClassifierGroupValidator
	{
		private readonly DbContext _db;

		public ClassifierGroupValidator(DbContext db)
		{
			_db = db;
		}

		public IList<ApiResultError> Errors { get; } = new List<ApiResultError>();

		public async Task<bool> ValidateInsert(ClassifierGroup item, CancellationToken cancellationToken)
		{
			await ValidateDuplicateCode(item, cancellationToken);

			return Errors.Count == 0;
		}

		public async Task<bool> ValidateUpdate(ClassifierGroup item, CancellationToken cancellationToken)
		{
			await ValidateDuplicateCode(item, cancellationToken);

			return Errors.Count == 0;
		}

		private async Task ValidateDuplicateCode(ClassifierGroup item, CancellationToken cancellationToken)
		{
			if (item == null) throw new ArgumentNullException(nameof(item));

			var duplicate = await (
					from parents in _db.GetTable<DbClassifierClosure>().Where(x => x.ChildUid == item.Uid)
					join root in _db.GetTable<DbClassifierGroup>().Where(x => x.ParentUid == null)
						on parents.ParentUid equals root.Uid
					join children in _db.GetTable<DbClassifierClosure>()
						on root.Uid equals children.ParentUid
					join child in _db.GetTable<DbClassifierGroup>()
						on children.ChildUid equals child.Uid
					select child)
				.Where(x => x.Uid != item.Uid && x.Code == item.Code)
				.FirstOrDefaultAsync(cancellationToken);

			if (duplicate != null)
			{
				Errors.Add(new ApiResultError
				{
					Key = "code",
					Messages = new[]
					{
						$"Код «{duplicate.Code}» уже используется в группе «{duplicate.Name}»."
					}
				});
			}
		}
	}
}
