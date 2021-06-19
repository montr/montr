using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Kompany.Impl.Entities;
using Montr.MasterData.Impl.Entities;
using Montr.Messages.Services;
using Montr.Tendr.Commands;
using Montr.Tendr.Impl.Entities;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.CommandHandlers
{
	public class SendInvitationsHandler : IRequestHandler<SendInvitations, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IAppUrlBuilder _appUrlBuilder;
		private readonly IEmailSender _emailSender;
		private readonly ITemplateRenderer _templateRenderer;

		public SendInvitationsHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			IDbContextFactory dbContextFactory,
			IAppUrlBuilder appUrlBuilder,
			IEmailSender emailSender,
			ITemplateRenderer templateRenderer)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_appUrlBuilder = appUrlBuilder;
			_emailSender = emailSender;
			_templateRenderer = templateRenderer;
		}

		public async Task<ApiResult> Handle(SendInvitations request, CancellationToken cancellationToken)
		{
			IList<InvitationMessageModel> invitations;

			using (var db = _dbContextFactory.Create())
			{
				// todo: remove reference to Montr.MasterData.Impl.Entities
				invitations = await (
					from e in db.GetTable<DbEvent>()
					join k in db.GetTable<DbCompany>() on e.CompanyUid equals k.Uid
					join kc in db.GetTable<DbClassifier>() on k.Uid equals kc.Uid
					join i in db.GetTable<DbInvitation>() on e.Uid equals i.EventUid
					join c in db.GetTable<DbClassifier>() on i.CounterpartyUid equals c.Uid
					where e.Uid == request.EventUid && i.Email != null
					select new InvitationMessageModel
					{
						EventNo = e.Id,
						EventName = e.Name,
						// todo: use IAppUrlBuilder
						// todo: use client routes
						EventUrl = "https://app.montr.io:5001/events/edit/" + e.Uid,
						CompanyName = kc.Name,
						CounterpartyName = c.Name,
						Email = i.Email
					}).ToListAsync(cancellationToken);
			}

			// todo: use transaction
			foreach (var invitation in invitations)
			{
				var message = await _templateRenderer.Render(MessageTemplateCode.EventInvitation, invitation, cancellationToken);

				// todo: send as separate task
				await _emailSender.Send(invitation.Email, message.Subject, message.Body, cancellationToken);
			}

			return new ApiResult { AffectedRows = invitations.Count };
		}
	}
}
