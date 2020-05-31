using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

			return await Render(template.Subject, template.Body, data, cancellationToken);
		}

		public async Task<Message> Render<TModel>(string subject, string body, TModel data, CancellationToken cancellationToken)
		{
			var stubble = new StubbleBuilder().Build();

			var renderedSubject = await stubble.RenderAsync(subject, data);
			var renderedBody = await stubble.RenderAsync(body, data);

			var renderedHtmlBody = Markdig.Markdown.ToHtml(renderedBody);

			return new Message
			{
				Subject = renderedSubject,
				Body = renderedHtmlBody
			};
		}
	}
}
