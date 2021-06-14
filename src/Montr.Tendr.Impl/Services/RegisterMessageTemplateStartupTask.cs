using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Messages.Models;
using Montr.Messages.Services;
using Montr.Tendr.Impl.CommandHandlers;
using Montr.Tendr.Models;

namespace Montr.Tendr.Impl.Services
{
	public class RegisterMessageTemplateStartupTask : IStartupTask
	{
		private readonly IMessageTemplateRegistrator _registrator;

		public RegisterMessageTemplateStartupTask(IMessageTemplateRegistrator registrator)
		{
			_registrator = registrator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			foreach (var item in GetMessageTemplates())
			{
				await _registrator.Register(item, cancellationToken);
			}
		}

		protected IEnumerable<MessageTemplate> GetMessageTemplates()
		{
			yield return new MessageTemplate
			{
				Uid = SendInvitationsHandler.TemplateUid,
				Code = MessageTemplateCodes.EventInvitation,
				Name = "Event invitation",
				IsSystem = true,
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
			};
		}
	}
}
