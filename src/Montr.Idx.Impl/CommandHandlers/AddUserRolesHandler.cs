using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Commands;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Impl.Services;

namespace Montr.Idx.Impl.CommandHandlers
{
	public class AddUserRolesHandler : IRequestHandler<AddUserRoles, ApiResult>
	{
		private readonly ILogger<AddUserRolesHandler> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly UserManager<DbUser> _userManager;

		public AddUserRolesHandler(ILogger<AddUserRolesHandler> logger,
			IUnitOfWorkFactory unitOfWorkFactory, UserManager<DbUser> userManager)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_userManager = userManager;
		}

		public async Task<ApiResult> Handle(AddUserRoles request, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			using (var scope = _unitOfWorkFactory.Create())
			{
				var dbUser = await _userManager.FindByIdAsync(request.UserUid.ToString());

				var identityResult =  await _userManager.AddToRolesAsync(dbUser, request.Roles);

				var result = identityResult.ToApiResult();

				if (result.Success)
				{
					_logger.LogInformation("Added user {userName} to roles {roles}.", dbUser.UserName, request.Roles);

					scope.Commit();
				}

				return result;
			}
		}
	}
}
