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
	public class ClassifierGroupValidator
	{
		private readonly DbContext _db;
		private readonly DbClassifierTree _tree;

		public ClassifierGroupValidator(DbContext db, DbClassifierTree tree)
		{
			_db = db;
			_tree = tree;
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
			var duplicate = await _db.GetTable<DbClassifierGroup>()
				.Where(x => x.TreeUid == _tree.Uid && x.Uid != item.Uid && x.Code == item.Code)
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
