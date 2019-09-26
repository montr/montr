using System;
using System.Threading.Tasks;
using Montr.Messages.Models;
using Montr.Messages.Services;
using Stubble.Core.Builders;

namespace Montr.Messages.Impl.Services
{
	public class MustacheTemplateRenderer : ITemplateRenderer
	{
		public async Task<Message> Render<TModel>(Guid templateUid, TModel data)
		{
			var template = GetTemplate(templateUid);

			var stubble = new StubbleBuilder().Build();

			return new Message
			{
				Subject = await stubble.RenderAsync(template.Subject, data),
				Body = await stubble.RenderAsync(template.Body, data)
			};
		}

		private MessageTemplate GetTemplate(Guid templateUid)
		{
			return new MessageTemplate
			{
				Uid = templateUid,
				Subject = "Персональное приглашение на Запрос предложений № {{EventNo}}",
				Body = @"
[LOGO]
<hr>

<h3>Здравствуйте!</h3>

<p>
<b>АО «ФЫВА-ЙЦУКЕН-ТЭК»</b> приглашает вас принять участие в торговой процедуре <b>Запрос предложений № {{EventNo}}</b>
</p>

<p>
<b>Предмет процедуры:</b><br>
{{invitation.EventName}}
</p>

<p>
Дата и время окончания приема заявок: <b>30.11.2018 15:00 MSK</b><br>
Дата и время рассмотрения заявок: <b>14.12.2018 15:00 MSK</b><br>
Дата и время подведения результатов процедуры: <b>31.12.2018 15:00 MSK</b><br>
</p>

<p>
Ознакомиться с описанием процедуры можно по адресу <a href=""{{invitation.EventUrl}}"">{{invitation.EventUrl}}</a>
</p>

<hr>
[CONTACTS]"
			};
		}
	}
}
