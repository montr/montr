using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Kompany.Impl.Entities;
using Montr.MasterData.Impl.Entities;
using Montr.Messages.Services;
using Montr.Metadata.Models;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class SendInvitationsHandler : IRequestHandler<SendInvitations, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IEmailSender _emailSender;
		private readonly ITemplateRenderer _templateRenderer;

		public SendInvitationsHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IEmailSender emailSender, ITemplateRenderer templateRenderer)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_emailSender = emailSender;
			_templateRenderer = templateRenderer;
		}

		public async Task<ApiResult> Handle(SendInvitations request, CancellationToken cancellationToken)
		{
			IList<InvitationMailModel> invitations;

			using (var db = _dbContextFactory.Create())
			{
				invitations = await (
					from e in db.GetTable<DbEvent>()
					join k in db.GetTable<DbCompany>() on e.CompanyUid equals k.Uid
					join i in db.GetTable<DbInvitation>() on e.Uid equals i.EventUid
					join c in db.GetTable<DbClassifier>() on i.CounterpartyUid equals c.Uid
					where e.Uid == request.EventUid && i.Email != null
					select new InvitationMailModel
					{
						EventNo = e.Id,
						EventName = e.Name,
						EventUrl = "http://app.tendr.montr.io:5010/events/edit/" + e.Uid + "/",
						CompanyName = k.Name,
						CounterpartyName = c.Name,
						Email = i.Email
					}).ToListAsync(cancellationToken);
			}

			var templateUid = Guid.Parse("4d3c920c-abfc-4f21-b900-6afb894413dd");

			foreach (var invitation in invitations)
			{
				var message = await _templateRenderer.Render(templateUid, invitation, cancellationToken);

				await _emailSender.Send(invitation.Email, message.Subject, message.Body);
			}

			return new ApiResult { AffectedRows = invitations.Count };
		}
	}
}
