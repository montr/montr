using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Data.Linq2Db;
using Montr.Docs.Impl.Entities;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.Docs.Impl.Services
{
	public class DbDocumentService : IDocumentService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly INumberGenerator _numberGenerator;

		public DbDocumentService(IDbContextFactory dbContextFactory, INumberGenerator numberGenerator)
		{
			_dbContextFactory = dbContextFactory;
			_numberGenerator = numberGenerator;
		}

		public async Task Create(Document document, CancellationToken cancellationToken)
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
					.Value(x => x.DocumentTypeUid, document.DocumentTypeUid)
					.Value(x => x.StatusCode, document.StatusCode)
					.Value(x => x.Direction, document.Direction.ToString())
					.Value(x => x.DocumentNumber, document.DocumentNumber)
					.Value(x => x.DocumentDate, document.DocumentDate)
					.Value(x => x.Name, document.Name)
					.InsertAsync(cancellationToken);
			}

			// todo: generate number, on publish(?)
			if (document.StatusCode == DocumentStatusCode.Published)
			{
				var documentNumber = await _numberGenerator
					.GenerateNumber(new GenerateNumberRequest
					{
						EntityTypeCode = DocumentType.EntityTypeCode,
						EntityTypeUid = DocumentType.CompanyRegistrationRequest,
						EntityUid = document.Uid
					}, cancellationToken);

				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbDocument>()
						.Where(x => x.Uid == document.Uid)
						.Set(x => x.DocumentNumber, documentNumber)
						.UpdateAsync(cancellationToken);
				}
			}
		}
	}
}
