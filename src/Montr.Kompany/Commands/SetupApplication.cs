using MediatR;
using Montr.Core.Models;
using Montr.Kompany.Models;

namespace Montr.Kompany.Commands
{
	public class SetupApplication : SetupModel, IRequest<ApiResult>
	{
	}
}
