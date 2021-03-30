using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class AddUserRolesHandler : IRequestHandler<AddUserRoles, ApiResult>
	{
		private readonly IUserManager _userManager;

		public AddUserRolesHandler(IUserManager userManager)
		{
			_userManager = userManager;
		}
		
		public async Task<ApiResult> Handle(AddUserRoles request, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();
			
			return await _userManager.AddRoles(request.UserUid, request.Roles, cancellationToken);
		}
	}
}
