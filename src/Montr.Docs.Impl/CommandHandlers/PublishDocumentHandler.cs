using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Docs.Commands;
using Montr.Docs.Impl.Entities;
using Montr.Docs.Models;

namespace Montr.Docs.Impl.CommandHandlers
{
	public class PublishDocumentHandler : IRequestHandler<PublishDocument, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public PublishDocumentHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(PublishDocument request, CancellationToken cancellationToken)
		{
			var documentUid = request.DocumentUid ?? throw new ArgumentNullException(nameof(request.DocumentUid));

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbDocument>()
						.Where(x => x.Uid == documentUid)
						.Set(x => x.StatusCode, DocumentStatusCode.Published)
						.UpdateAsync(cancellationToken);
				}

				var success = affected == 1;

				if (success) scope.Commit();

				return new ApiResult { Success = success, AffectedRows = affected };
			}
		}
	}
}
