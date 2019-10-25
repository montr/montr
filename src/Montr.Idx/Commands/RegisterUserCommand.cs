using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class RegisterUserCommand : RegisterUserModel, IRequest<ApiResult>
	{
	}
}
