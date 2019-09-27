using System;
using System.Threading.Tasks;
using Markdig;
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

			var subject = await stubble.RenderAsync(template.Subject, data);
			var body = await stubble.RenderAsync(template.Body, data);

			return new Message
			{
				Subject = subject,
				Body = Markdown.ToHtml(body) 
			};
		}

		// https://commonmark.org/help/
		private MessageTemplate GetTemplate(Guid templateUid)
		{
			return new MessageTemplate
			{
				Uid = templateUid,
				Subject = "🔥 Персональное приглашение на Запрос предложений № {{EventNo}}",
				Body = @"
![](https://dev.montr.net/favicon.ico)

### Здравствуйте!

**АО «ФЫВА-ЙЦУКЕН-ТЭК»** приглашает вас принять участие в торговой процедуре **Запрос предложений № {{EventNo}}**

**Предмет процедуры:**
{{invitation.EventName}}

Дата и время окончания приема заявок: **30.11.2018 15:00 MSK**   
Дата и время рассмотрения заявок: **14.12.2018 15:00 MSK**   
Дата и время подведения результатов процедуры: **31.12.2018 15:00 MSK**   

Ознакомиться с описанием процедуры можно по адресу <{{EventUrl}}>

___

[CONTACTS]"
			};
		}
	}
}
