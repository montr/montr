using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class Register : RegisterModel, IRequest<ApiResult>
	{
	}
}
