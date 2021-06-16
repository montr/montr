using System.Threading;
using System.Threading.Tasks;
using Montr.Messages.Models;

namespace Montr.Messages.Services
{
	public interface ITemplateRenderer
	{
		Task<Message> Render<TModel>(string templateCode, TModel data, CancellationToken cancellationToken);

		Task<Message> Render<TModel>(string subject, string body, TModel data, CancellationToken cancellationToken);
	}
}
