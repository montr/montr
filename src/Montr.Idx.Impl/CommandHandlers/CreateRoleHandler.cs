using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Idx.Commands;
using Montr.Idx.Models;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class CreateRoleHandler : IRequestHandler<CreateRole, Role>
	{
		public Task<Role> Handle(CreateRole request, CancellationToken cancellationToken)
		{
			var role = new Role();

			return Task.FromResult(role);
		}
	}
}
