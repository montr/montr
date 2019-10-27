using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class LogoutCommand : IRequest<ApiResult>
	{
	}
}
