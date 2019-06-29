using System;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Data.Linq2Db;
using Montr.Dokumento.Impl.Entities;
using Montr.Dokumento.Models;
using Montr.Dokumento.Services;

namespace Montr.Dokumento.Impl.Services
{
	public class DbDocumentRepository: IDocumentRepository
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbDocumentRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task Create(Document document)
		{
			if (document.Uid == Guid.Empty)
				document.Uid = Guid.NewGuid();

			if (document.StatusCode == null)
				document.StatusCode = DocumentStatusCode.Draft;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbDocument>()
					.Value(x => x.Uid, document.Uid)
					.Value(x => x.CompanyUid, document.CompanyUid)
					.Value(x => x.ConfigCode, document.ConfigCode)
					.Value(x => x.StatusCode, document.StatusCode)
					.InsertAsync();
			}
		}
	}
}
