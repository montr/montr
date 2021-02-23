using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Idx.Commands;
using Montr.Idx.Models;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class CreateUserHandler : IRequestHandler<CreateUser, User>
	{
		public Task<User> Handle(CreateUser request, CancellationToken cancellationToken)
		{
			var user = new User();

			return Task.FromResult(user);
		}
	}
}
