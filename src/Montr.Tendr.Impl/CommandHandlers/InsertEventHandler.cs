using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class InsertEventHandler : IRequestHandler<InsertEvent, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public InsertEventHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ApiResult> Handle(InsertEvent request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					DbEvent template = null;

					if (item.TemplateUid != null)
					{
						template = await db.GetTable<DbEvent>()
							.Where(x => x.Uid == item.TemplateUid)
							.SingleOrDefaultAsync(cancellationToken);
					}

					var uid = Guid.NewGuid();

					// todo: use number generation service
					var id = db.SelectSequenceNextValue<long>("event_id_seq");

					await db.GetTable<DbEvent>()
						.Value(x => x.Uid, uid)
						.Value(x => x.Id, id)
						.Value(x => x.CompanyUid, request.CompanyUid)
						.Value(x => x.IsTemplate, false)
						.Value(x => x.TemplateUid, template?.Uid)
						.Value(x => x.Name, template?.Name)
						.Value(x => x.Description, template?.Description)
						.Value(x => x.ConfigCode, template?.ConfigCode) // todo: remove
						.Value(x => x.StatusCode, EventStatusCode.Draft)
						.InsertAsync(cancellationToken);

					if (template != null)
					{
						var invitations = await db.GetTable<DbInvitation>()
							.Where(x => x.EventUid == template.Uid)
							.Select(x => new DbInvitation
							{
								Uid = Guid.NewGuid(),
								EventUid = uid,
								StatusCode = InvitationStatusCode.Draft,
								CounterpartyUid = x.CounterpartyUid,
								Email = x.Email
							}).ToListAsync(cancellationToken);

						if (invitations.Count > 0)
						{
							await db.BulkCopyAsync(invitations, cancellationToken);
						}
					}

					scope.Commit();

					return new ApiResult { Uid = uid };
				}
			}
		}
	}
}
