using MediatR;
using Montr.Core.Models;
using Montr.Kompany.Models;

namespace Montr.Kompany.Commands
{
	public class SetupSystem : SetupSystemModel, IRequest<ApiResult>
	{
	}
}
