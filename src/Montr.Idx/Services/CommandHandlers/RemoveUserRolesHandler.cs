using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Entities;
using Montr.Idx.Services.Implementations;

namespace Montr.Idx.Services.CommandHandlers
{
	public class RemoveUserRolesHandler : IRequestHandler<RemoveUserRoles, ApiResult>
	{
		private readonly ILogger<RemoveUserRolesHandler> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly UserManager<DbUser> _userManager;

		public RemoveUserRolesHandler(ILogger<RemoveUserRolesHandler> logger,
			IUnitOfWorkFactory unitOfWorkFactory, UserManager<DbUser> userManager)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(RemoveUserRoles request, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = _unitOfWorkFactory.Create())
			{
				var dbUser = await _userManager.FindByIdAsync(request.UserUid.ToString());

				var identityResult =  await _userManager.RemoveFromRolesAsync(dbUser, request.Roles);

				var result = identityResult.ToApiResult();

				if (result.Success)
				{
					_logger.LogInformation("Removed user {userName} from roles {roles}.", dbUser.UserName, request.Roles);

					scope.Commit();
				}

				return result;
			}
		}
	}
}
