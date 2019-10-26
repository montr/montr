using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class ConfirmEmailCommand : IRequest<ApiResult>
	{
		public string UserId { get; set; }

		public string Code { get; set; }
	}
}
