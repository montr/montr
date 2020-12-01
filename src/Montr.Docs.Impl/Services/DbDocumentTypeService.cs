using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Impl.Entities;
using Montr.Docs.Models;
using Montr.Docs.Services;

namespace Montr.Docs.Impl.Services
{
	public class DbDocumentTypeService : IDocumentTypeService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<DocumentType> _repository;

		public DbDocumentTypeService(IDbContextFactory dbContextFactory,  IRepository<DocumentType> repository)
		{
			_dbContextFactory = dbContextFactory;
			_repository = repository;
		}

		public async Task<DocumentType> TryGet(string typeCode, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new DocumentTypeSearchRequest
			{
				Code = typeCode ?? throw new ArgumentNullException(nameof(typeCode)),
				PageNo = 0,
				PageSize = 1,
				SkipPaging = true
			}, cancellationToken);

			return result.Rows.SingleOrDefault();
		}

		public async Task<DocumentType> Get(string typeCode, CancellationToken cancellationToken)
		{
			var result = await TryGet(typeCode, cancellationToken);

			if (result == null)
			{
				throw new InvalidOperationException($"Document type \"{typeCode}\" not found.");
			}

			return result;
		}

		public async Task<DocumentType> TryGet(Guid uid, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new DocumentTypeSearchRequest
			{
				Uid = uid,
				PageNo = 0,
				PageSize = 1,
				SkipPaging = true
			}, cancellationToken);

			return result.Rows.SingleOrDefault();
		}

		public async Task<DocumentType> Get(Guid uid, CancellationToken cancellationToken)
		{
			var result = await TryGet(uid, cancellationToken);

			if (result == null)
			{
				throw new InvalidOperationException($"Document type \"{uid}\" not found.");
			}

			return result;
		}

		public async Task<ApiResult> Insert(DocumentType item, CancellationToken cancellationToken)
		{
			var itemUid = Guid.NewGuid();

			// todo: validation and limits
			// todo: reserved codes (add, new etc. can conflict with routing)

			using (var db = _dbContextFactory.Create())
			{
				/*var validator = new DocumentTypeValidator(db);

				if (await validator.ValidateInsert(item, cancellationToken) == false)
				{
					return new ApiResult { Success = false, Errors = validator.Errors };
				}*/

				// todo: change date

				await db.GetTable<DbDocumentType>()
					.Value(x => x.Uid, itemUid)
					.Value(x => x.Code, item.Code)
					.Value(x => x.Name, item.Name)
					.Value(x => x.Description, item.Description)
					.InsertAsync(cancellationToken);
			}

			return new ApiResult { Uid = itemUid };
		}
	}
}
