﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Commands;
using Montr.Metadata.Impl.Entities;

namespace Montr.Metadata.Impl.CommandHandlers
{
	public class DeleteDataFieldHandler : IRequestHandler<DeleteDataField, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public DeleteDataFieldHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(DeleteDataField request, CancellationToken cancellationToken)
		{
			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbFieldMetadata>()
						.Where(x => x.EntityTypeCode == request.EntityTypeCode &&
									x.EntityUid == request.EntityUid &&
									request.Uids.Contains(x.Uid))
						.DeleteAsync(cancellationToken);
				}

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
