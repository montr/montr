using MediatR;
using Montr.Core.Models;
using Montr.Messages.Models;

namespace Montr.Messages.Commands
{
	public class RegisterMessageTemplate : IRequest<ApiResult>
	{
		public MessageTemplate Item { get; set; }
	}
}
