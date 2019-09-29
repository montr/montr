using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Metadata.Models;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class InsertEventHandler : IRequestHandler<InsertEvent, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;

		public InsertEventHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
		}

		public async Task<ApiResult> Handle(InsertEvent request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var now = _dateTimeProvider.GetUtcNow();

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var uid = Guid.NewGuid();

					// todo: use number generation service
					var id = db.SelectSequenceNextValue<long>("event_id_seq");

					await db.GetTable<DbEvent>()
						.Value(x => x.Uid, uid)
						.Value(x => x.Id, id)
						.Value(x => x.CompanyUid, request.CompanyUid)
						// todo: is it possible to create event without templates, from scratch?
						.Value(x => x.IsTemplate, false)
						.Value(x => x.TemplateUid, item.TemplateUid)
						.Value(x => x.ConfigCode, item.ConfigCode)
						.Value(x => x.StatusCode, EventStatusCode.Draft)
						.Value(x => x.Name, item.Name)
						.Value(x => x.Description, item.Description)
						.InsertAsync(cancellationToken);

					scope.Commit();

					return new ApiResult { Uid = uid };
				}
			}
		}
	}
}
