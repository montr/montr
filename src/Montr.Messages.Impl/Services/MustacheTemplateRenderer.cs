using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Markdig;
using Montr.Core.Services;
using Montr.Messages.Models;
using Montr.Messages.Services;
using Stubble.Core.Builders;

namespace Montr.Messages.Impl.Services
{
	public class MustacheTemplateRenderer : ITemplateRenderer
	{
		private readonly IRepository<MessageTemplate> _repository;

		public MustacheTemplateRenderer(IRepository<MessageTemplate> repository)
		{
			_repository = repository;
		}

		public async Task<Message> Render<TModel>(Guid templateUid, TModel data, CancellationToken cancellationToken)
		{
			var templateSearchResult = await _repository
				.Search(new MessageTemplateSearchRequest { Uid = templateUid }, cancellationToken);

			var template = templateSearchResult.Rows.SingleOrDefault();

			if (template == null)
			{
				throw new InvalidOperationException($"Template {templateUid} not found.");
			}

			var stubble = new StubbleBuilder().Build();

			var subject = await stubble.RenderAsync(template.Subject, data);
			var body = await stubble.RenderAsync(template.Body, data);

			return new Message
			{
				Subject = subject,
				Body = Markdown.ToHtml(body) 
			};
		}
	}
}
