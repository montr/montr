using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Messages.Models;
using Montr.Messages.Services;
using Stubble.Core.Builders;

namespace Montr.Messages.Impl.Services
{
	public class MustacheTemplateRenderer : ITemplateRenderer
	{
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;

		public MustacheTemplateRenderer(INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory)
		{
			_classifierRepositoryFactory = classifierRepositoryFactory;
		}

		public async Task<Message> Render<TModel>(string templateCode, TModel data, CancellationToken cancellationToken)
		{
			var repository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.MessageTemplate);

			var template = (MessageTemplate)await repository.Get(ClassifierTypeCode.MessageTemplate, templateCode, cancellationToken);

			if (template == null)
			{
				throw new InvalidOperationException($"Template {templateCode} not found.");
			}

			return await Render(template.Subject, template.Body, data, cancellationToken);
		}

		public async Task<Message> Render<TModel>(string subject, string body, TModel data, CancellationToken cancellationToken)
		{
			var stubble = new StubbleBuilder().Build();

			var renderedSubject = subject != null ? await stubble.RenderAsync(subject, data) : null;
			var renderedBody = body != null ? await stubble.RenderAsync(body, data) : null;

			var renderedHtmlBody = renderedBody != null ? Markdig.Markdown.ToHtml(renderedBody) : null;

			return new Message
			{
				Subject = renderedSubject,
				Body = renderedHtmlBody
			};
		}
	}
}
