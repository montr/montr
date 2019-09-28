using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Markdig;
using Montr.Data.Linq2Db;
using Montr.Messages.Impl.Entities;
using Montr.Messages.Models;
using Montr.Messages.Services;
using Stubble.Core.Builders;

namespace Montr.Messages.Impl.Services
{
	public class MustacheTemplateRenderer : ITemplateRenderer
	{
		private readonly IDbContextFactory _dbContextFactory;

		public MustacheTemplateRenderer(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<Message> Render<TModel>(Guid templateUid, TModel data, CancellationToken cancellationToken)
		{
			var template = await GetTemplate(templateUid, cancellationToken);

			var stubble = new StubbleBuilder().Build();

			var subject = await stubble.RenderAsync(template.Subject, data);
			var body = await stubble.RenderAsync(template.Body, data);

			return new Message
			{
				Subject = subject,
				Body = Markdown.ToHtml(body) 
			};
		}

		private async Task< MessageTemplate> GetTemplate(Guid templateUid, CancellationToken cancellationToken)
		{
			using (var db = _dbContextFactory.Create())
			{
				return await db
					.GetTable<DbMessageTemplate>()
					.Where(x => x.Uid == templateUid)
					.Select(x => new MessageTemplate
					{
						Uid = x.Uid,
						Subject = x.Subject,
						Body = x.Body
					})
					.SingleAsync(cancellationToken);
			}
		}
	}
}
