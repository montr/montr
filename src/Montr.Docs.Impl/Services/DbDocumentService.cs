using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Events;
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
		private readonly IMediator _mediator;

		public DbDocumentService(IDbContextFactory dbContextFactory, INumberGenerator numberGenerator, IMediator mediator)
		{
			_dbContextFactory = dbContextFactory;
			_numberGenerator = numberGenerator;
			_mediator = mediator;
		}

		public async Task Create(Document document, CancellationToken cancellationToken)
		{
			if (document.Uid == Guid.Empty)
				document.Uid = Guid.NewGuid();

			if (document.StatusCode == null)
			{
				document.StatusCode = DocumentStatusCode.Draft;
			}

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

			if (document.StatusCode != DocumentStatusCode.Draft)
			{
				await _mediator.Publish(new EntityStatusChanged<Document>
				{
					Entity = document,
					StatusCode = DocumentStatusCode.Draft
				}, cancellationToken);
			}

			// todo: generate number, on publish(?)
			if (document.StatusCode == DocumentStatusCode.Published)
			{
				document.DocumentNumber = await _numberGenerator
					.GenerateNumber(new GenerateNumberRequest
					{
						EntityTypeCode = DocumentType.EntityTypeCode,
						EntityTypeUid = document.DocumentTypeUid,
						EntityUid = document.Uid
					}, cancellationToken);

				using (var db = _dbContextFactory.Create())
				{
					await db.GetTable<DbDocument>()
						.Where(x => x.Uid == document.Uid)
						.Set(x => x.DocumentNumber, document.DocumentNumber)
						.UpdateAsync(cancellationToken);
				}
			}

			await _mediator.Publish(new EntityStatusChanged<Document>
			{
				Entity = document,
				StatusCode = document.StatusCode
			}, cancellationToken);
		}
	}
}
