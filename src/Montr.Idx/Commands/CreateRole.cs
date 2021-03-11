using MediatR;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class CreateRole : IRequest<Role>
	{
	}
}
