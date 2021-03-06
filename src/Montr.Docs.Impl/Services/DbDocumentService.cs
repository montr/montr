﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Events;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Commands;
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
		private readonly IPublisher _mediator;

		public DbDocumentService(IDbContextFactory dbContextFactory, INumberGenerator numberGenerator, IPublisher mediator)
		{
			_dbContextFactory = dbContextFactory;
			_numberGenerator = numberGenerator;
			_mediator = mediator;
		}

		public async Task<SearchResult<Document>> Search(DocumentSearchRequest request, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				// todo: add indexes
				var query = db.GetTable<DbDocument>().AsQueryable();

				if (request.Uid.HasValue)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				if (request.UserUid.HasValue)
				{
					query = query.Where(x => x.CreatedBy == request.UserUid);
				}

				var data = await query
					.Apply(request, x => x.DocumentDate, SortOrder.Descending)
					.Select(x => Materialize(x))
					.ToListAsync(cancellationToken);

				return new SearchResult<Document>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		protected virtual Document Materialize(DbDocument dbItem)
		{
			return new Document
			{
				Uid = dbItem.Uid,
				DocumentTypeUid = dbItem.DocumentTypeUid,
				StatusCode = dbItem.StatusCode,
				Direction = Enum.Parse<DocumentDirection>(dbItem.Direction, true),
				DocumentDate = dbItem.DocumentDate,
				DocumentNumber = dbItem.DocumentNumber,
				CompanyUid = dbItem.CompanyUid,
				Name = dbItem.Name,
				CreatedAtUtc = dbItem.CreatedAtUtc,
				CreatedBy = dbItem.CreatedBy,
				ModifiedAtUtc = dbItem.ModifiedAtUtc,
				ModifiedBy = dbItem.ModifiedBy,
				Url = $"/documents/edit/{dbItem.Uid}"
			};
		}

		public async Task<ApiResult> Create(Document document, CancellationToken cancellationToken)
		{
			if (document.DocumentDate == DateTime.MinValue) throw new InvalidOperationException("Invalid document date (cannot be min datetime)");

			document.Uid ??= Guid.NewGuid();
			document.StatusCode ??= DocumentStatusCode.Draft;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbDocument>()
					.Value(x => x.Uid, document.Uid)
					.Value(x => x.DocumentTypeUid, document.DocumentTypeUid)
					.Value(x => x.StatusCode, document.StatusCode)
					.Value(x => x.Direction, document.Direction.ToString().ToLowerInvariant())
					.Value(x => x.DocumentNumber, document.DocumentNumber)
					.Value(x => x.DocumentDate, document.DocumentDate)
					.Value(x => x.CompanyUid, document.CompanyUid)
					.Value(x => x.Name, document.Name)
					.Value(x => x.CreatedAtUtc, document.CreatedAtUtc)
					.Value(x => x.CreatedBy, document.CreatedBy)
					.Value(x => x.Name, document.Name)
					.InsertAsync(cancellationToken);
			}

			/*if (document.StatusCode != DocumentStatusCode.Draft)
			{
				await _mediator.Publish(new EntityStatusChanged<Document>
				{
					Entity = document,
					StatusCode = DocumentStatusCode.Draft
				}, cancellationToken);
			}*/

			if (document.StatusCode == DocumentStatusCode.Published)
			{
				var documentNumber = await _numberGenerator
					.GenerateNumber(new GenerateNumberRequest
					{
						EntityTypeCode = DocumentType.EntityTypeCode,
						EntityTypeUid = document.DocumentTypeUid,
						EntityUid = document.Uid
					}, cancellationToken);

				if (documentNumber != null)
				{
					document.DocumentNumber = documentNumber;

					using (var db = _dbContextFactory.Create())
					{
						await db.GetTable<DbDocument>()
							.Where(x => x.Uid == document.Uid)
							.Set(x => x.DocumentNumber, documentNumber)
							.UpdateAsync(cancellationToken);
					}
				}
			}

			await _mediator.Publish(new EntityStatusChanged<Document>
			{
				Entity = document,
				StatusCode = document.StatusCode
			}, cancellationToken);

			return new ApiResult { Uid = document.Uid };
		}

		public virtual async Task<ApiResult> Delete(DeleteDocument request, CancellationToken cancellationToken)
		{
			ApiResult result;

			using (var db = _dbContextFactory.Create())
			{
				result = await DeleteInternal(db, request, cancellationToken);

				if (result.Success == false) return result;
			}

			return result;
		}

		protected virtual async Task<ApiResult> DeleteInternal(
			DbContext db, DeleteDocument request, CancellationToken cancellationToken = default)
		{
			var affected = await db.GetTable<DbDocument>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			return new ApiResult { AffectedRows = affected };
		}
	}
}
