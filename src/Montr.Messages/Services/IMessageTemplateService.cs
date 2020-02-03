using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Messages.Models;

namespace Montr.Messages.Services
{
	public interface IMessageTemplateService
	{
		Task<MessageTemplate> TryGet(Guid uid, CancellationToken cancellationToken);

		Task<MessageTemplate> Get(Guid uid, CancellationToken cancellationToken);

		Task<ApiResult> Insert(MessageTemplate item, CancellationToken cancellationToken);
	}
}
