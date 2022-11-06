using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Entities;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.CommandHandlers
{
	public class ClassifierTypeValidator
	{
		private readonly DbContext _db;

		public ClassifierTypeValidator(DbContext db)
		{
			_db = db;
		}

		public IList<ApiResultError> Errors { get; } = new List<ApiResultError>();

		public async Task<bool> ValidateInsert(ClassifierType item, CancellationToken cancellationToken)
		{
			await ValidateDuplicateCode(item, cancellationToken);

			return Errors.Count == 0;
		}

		public async Task<bool> ValidateUpdate(ClassifierType item, CancellationToken cancellationToken)
		{
			await ValidateDuplicateCode(item, cancellationToken);

			return Errors.Count == 0;
		}

		private async Task ValidateDuplicateCode(ClassifierType item, CancellationToken cancellationToken)
		{
			var duplicate = await _db.GetTable<DbClassifierType>()
				.Where(x => x.Uid != item.Uid && x.Code == item.Code)
				.FirstOrDefaultAsync(cancellationToken);

			if (duplicate != null)
			{
				Errors.Add(new ApiResultError
				{
					Key = "code",
					Messages = new[]
					{
						$"Код «{duplicate.Code}» уже используется в классификаторе «{duplicate.Name}»."
					}
				});
			}
		}
	}
}
