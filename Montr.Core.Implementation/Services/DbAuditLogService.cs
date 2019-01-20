using System;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Implementation.Entities;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Implementation.Services
{
	public class DbAuditLogService : IAuditLogService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IJsonSerializer _jsonSerializer;

		public DbAuditLogService(IDbContextFactory dbContextFactory, IJsonSerializer jsonSerializer)
		{
			_dbContextFactory = dbContextFactory;
			_jsonSerializer = jsonSerializer;
		}

		public async Task Save(AuditEvent entry)
		{
			using (var db = _dbContextFactory.Create())
			{
				var insertable = db.GetTable<DbAuditLog>()
					.Value(x => x.Uid, Guid.NewGuid())
					.Value(x => x.EntityTypeCode, entry.EntityTypeCode)
					.Value(x => x.EntityUid, entry.EntityUid)
					.Value(x => x.CompanyUid, entry.CompanyUid)
					.Value(x => x.UserUid, entry.UserUid)
					.Value(x => x.CreatedAtUtc, entry.CreatedAtUtc)
					.Value(x => x.MessageCode, entry.MessageCode)
					.Value(x => x.Ip, entry.Ip)
					.Value(x => x.Browser, entry.Browser);

				if (entry.MessageParameters != null)
				{
					insertable = insertable
						.Value(x=> x.MessageParameters, _jsonSerializer.Serialize(entry.MessageParameters));
				}

				await insertable.InsertAsync();
			}
		}
	}
}
