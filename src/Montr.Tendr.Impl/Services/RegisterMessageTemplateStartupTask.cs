using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.Logging;
using Montr.Messages.Commands;
using Montr.Messages.Models;
using Montr.Messages.Services;
using Montr.Tendr.Impl.CommandHandlers;

namespace Montr.Tendr.Impl.Services
{
	public class RegisterMessageTemplateStartupTask : AbstractRegisterMessageTemplateStartupTask
	{
		public RegisterMessageTemplateStartupTask(ILogger<RegisterMessageTemplateStartupTask> logger, IMediator mediator) : base(logger, mediator)
		{
		}

		protected override IEnumerable<RegisterMessageTemplate> GetCommands()
		{
			yield return new RegisterMessageTemplate
			{
				Item = new MessageTemplate
				{
					Uid = SendInvitationsHandler.TemplateUid,
					Subject = "🔥 Персональное приглашение на Запрос предложений № {{EventNo}}",
					Body = @"
![](https://dev.montr.net/favicon.ico)

### Здравствуйте!

**{{CompanyName}}** приглашает вас принять участие в торговой процедуре **Запрос предложений № {{EventNo}}**

**Предмет процедуры:**
{{EventName}}

Дата и время окончания приема заявок: **30.11.2018 15:00 MSK**   
Дата и время рассмотрения заявок: **14.12.2018 15:00 MSK**   
Дата и время подведения результатов процедуры: **31.12.2018 15:00 MSK**   

Ознакомиться с описанием процедуры можно по адресу <{{EventUrl}}>
"
				}
			};
		}
	}
}
