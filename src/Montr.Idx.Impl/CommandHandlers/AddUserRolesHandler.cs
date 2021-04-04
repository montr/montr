using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class AddUserRolesHandler : IRequestHandler<AddUserRoles, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IUserManager _userManager;

		public AddUserRolesHandler(IUnitOfWorkFactory unitOfWorkFactory, IUserManager userManager)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(AddUserRoles request, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _userManager.AddRoles(request.UserUid, request.Roles, cancellationToken);

				if (result.Success)
				{
					scope.Commit();
				}

				return result;
			}
		}
	}
}
