using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Idx.Models;
using Montr.Idx.Queries;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetUserHandler : IRequestHandler<GetUser, User>
	{
		private readonly IUserManager _userManager;

		public GetUserHandler(IUserManager userManager)
		{
			_userManager = userManager;
		}

		public async Task<User> Handle(GetUser request, CancellationToken cancellationToken)
		{
			return await _userManager.Get(request.Uid, cancellationToken);
		}
	}
}
