using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class ClassifierValidator
	{
		private readonly DbContext _db;
		private readonly ClassifierType _type;

		public ClassifierValidator(DbContext db, ClassifierType type)
		{
			_db = db;
			_type = type;
		}

		public IList<ApiResultError> Errors { get; } = new List<ApiResultError>();

		public async Task<bool> ValidateInsert(Classifier item, CancellationToken cancellationToken)
		{
			await ValidateDuplicateCode(item, cancellationToken);

			return Errors.Count == 0;
		}

		public async Task<bool> ValidateUpdate(Classifier item, CancellationToken cancellationToken)
		{
			await ValidateDuplicateCode(item, cancellationToken);

			return Errors.Count == 0;
		}

		private async Task ValidateDuplicateCode(Classifier item, CancellationToken cancellationToken)
		{
			var duplicate = await _db.GetTable<DbClassifier>()
				.Where(x => x.TypeUid == _type.Uid && x.Uid != item.Uid && x.Code == item.Code)
				.FirstOrDefaultAsync(cancellationToken);

			if (duplicate != null)
			{
				Errors.Add(new ApiResultError
				{
					Key = "code",
					Messages = new[]
					{
						$"Код «{duplicate.Code}» уже используется в элементе «{duplicate.Name}»."
					}
				});
			}
		}
	}
}
