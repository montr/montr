using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Messages.Models;

namespace Montr.Messages.Services
{
	public interface ITemplateRenderer
	{
		Task<Message> Render<TModel>(Guid templateUid, TModel data, CancellationToken cancellationToken);

		Task<Message> Render<TModel>(string subject, string body, TModel data, CancellationToken cancellationToken);
	}
}
