using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Messages.Models;

namespace Montr.Messages.Services
{
	public interface IMessageTemplateRegistrator
	{
		Task<ApiResult> Register(MessageTemplate item, CancellationToken cancellationToken);
	}
}
