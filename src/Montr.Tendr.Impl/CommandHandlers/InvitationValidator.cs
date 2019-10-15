using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Data.Linq2Db;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class InvitationValidator
	{
		private readonly DbContext _db;
		private readonly Guid _eventUid;

		public InvitationValidator(DbContext db, Guid eventUid)
		{
			_db = db;
			_eventUid = eventUid;
		}

		public IList<ApiResultError> Errors { get; } = new List<ApiResultError>();

		public async Task<bool> ValidateUpdate( Invitation item, CancellationToken cancellationToken)
		{
			await ValidateDuplicateCounterparty(item, cancellationToken);

			return Errors.Count == 0;
		}

		private async Task ValidateDuplicateCounterparty(Invitation item, CancellationToken cancellationToken)
		{
			var duplicate = await _db.GetTable<DbInvitation>()
				.Where(x => x.EventUid == _eventUid
							&& x.CounterpartyUid == item.CounterpartyUid
				            && x.Uid != item.Uid)
				.FirstOrDefaultAsync(cancellationToken);

			if (duplicate != null)
			{
				Errors.Add(new ApiResultError
				{
					// todo: get key from metadata or expression
					Key = "counterpartyUid",
					Messages = new[]
					{
						"Выбранный контрагент уже приглашен на событие."
					}
				});
			}
		}
	}
}
