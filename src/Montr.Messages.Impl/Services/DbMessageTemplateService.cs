using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Messages.Impl.Entities;
using Montr.Messages.Models;
using Montr.Messages.Services;

namespace Montr.Messages.Impl.Services
{
	public class DbMessageTemplateService : IMessageTemplateService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<MessageTemplate> _repository;

		public DbMessageTemplateService(
			IDbContextFactory dbContextFactory,
			IRepository<MessageTemplate> repository)
		{
			_dbContextFactory = dbContextFactory;
			_repository = repository;
		}

		// MessageTemplateSearchRequest
		public async Task<MessageTemplate> TryGet(Guid uid, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new MessageTemplateSearchRequest
			{
				Uid = uid,
				PageNo = 0,
				PageSize = 1,
				SkipPaging = true
			}, cancellationToken);

			return result.Rows.SingleOrDefault();
		}

		public async Task<MessageTemplate> Get(Guid uid, CancellationToken cancellationToken)
		{
			var result = await TryGet(uid, cancellationToken);

			if (result == null)
			{
				throw new InvalidOperationException($"Message template with uid \"{uid}\" not found.");
			}

			return result;
		}

		public async Task<ApiResult> Insert(MessageTemplate item, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbMessageTemplate>()
					.Value(x => x.Uid, item.Uid)
					.Value(x => x.Subject, item.Subject)
					.Value(x => x.Body, item.Body)
					.InsertAsync(cancellationToken);
			}

			return new ApiResult { Uid = item.Uid };
		}
	}
}
