using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
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

		public SendInvitationsHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory, IEmailSender emailSender)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_emailSender = emailSender;
		}

		public async Task<ApiResult> Handle(SendInvitations request, CancellationToken cancellationToken)
		{
			IList<InvitationMailModel> invitations;

			using (var db = _dbContextFactory.Create())
			{
				invitations = await (
					from e in db.GetTable<DbEvent>()
					join i in db.GetTable<DbInvitation>() on e.Uid equals i.EventUid
					join c in db.GetTable<DbClassifier>() on i.CounterpartyUid equals c.Uid
					where e.Uid == request.EventUid && i.Email != null
					select new InvitationMailModel
					{
						EventNo = e.Id,
						EventName = e.Name,
						EventUrl = "http://app.tendr.montr.io:5010/events/edit/" + e.Uid + "/",
						CounterpartyName = c.Name,
						Email = i.Email
					}).ToListAsync(cancellationToken);
			}

			foreach (var invitation in invitations)
			{
				await _emailSender.Send(invitation.Email,
					$"Персональное приглашение на Запрос предложений № {invitation.EventNo}",
					$@"
[LOGO]
<hr>

<h3>Здравствуйте!</h3>

<p>
<b>АО «ФЫВА-ЙЦУКЕН-ТЭК»</b> приглашает вас принять участие в торговой процедуре <b>Запрос предложений № {invitation.EventNo}</b>
</p>

<p>
<b>Предмет процедуры:</b><br>
{invitation.EventName}
</p>

<p>
Дата и время окончания приема заявок: <b>30.11.2018 15:00 MSK</b><br>
Дата и время рассмотрения заявок: <b>14.12.2018 15:00 MSK</b><br>
Дата и время подведения результатов процедуры: <b>31.12.2018 15:00 MSK</b><br>
</p>

<p>
Ознакомиться с описанием процедуры можно по адресу <a href=""{invitation.EventUrl}"">{invitation.EventUrl}</a>
</p>

<hr>
[CONTACTS]");
			}

			return new ApiResult { AffectedRows = invitations.Count };
		}
	}
}
