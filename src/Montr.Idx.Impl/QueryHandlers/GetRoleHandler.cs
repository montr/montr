using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Idx.Models;
using Montr.Idx.Queries;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetRoleHandler : IRequestHandler<GetRole, Role>
	{
		private readonly IRoleManager _roleManager;

		public GetRoleHandler(IRoleManager roleManager)
		{
			_roleManager = roleManager;
		}

		public async Task<Role> Handle(GetRole request, CancellationToken cancellationToken)
		{
			return await _roleManager.Get(request.Uid, cancellationToken);
		}
	}
}