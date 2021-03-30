using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class RemoveUserRolesHandler : IRequestHandler<RemoveUserRoles, ApiResult>
	{
		private readonly IUserManager _userManager;

		public RemoveUserRolesHandler(IUserManager userManager)
		{
			_userManager = userManager;
		}
		
		public async Task<ApiResult> Handle(RemoveUserRoles request, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			return await _userManager.RemoveRoles(request.UserUid, request.Roles, cancellationToken);
		}
	}
}
