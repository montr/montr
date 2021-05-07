using System;
using System.Linq;
using System.Security.Claims;
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
	public class ManageRolePermissionsHandler : IRequestHandler<ManageRolePermissions, ApiResult>
	{
		private readonly ILogger<ManageRolePermissionsHandler> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly RoleManager<DbRole> _roleManager;

		public ManageRolePermissionsHandler(ILogger<ManageRolePermissionsHandler> logger,
			IUnitOfWorkFactory unitOfWorkFactory, RoleManager<DbRole> roleManager)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_roleManager = roleManager;
		}

		public async Task<ApiResult> Handle(ManageRolePermissions request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var roleUid = request?.RoleUid ?? throw new ArgumentNullException(nameof(request));

			var dbRole = await _roleManager.FindByIdAsync(roleUid.ToString());

			var existingClaims = (await _roleManager.GetClaimsAsync(dbRole))
				.Where(x => x.Type == Permission.ClaimType)
				.Select(x => x.Value);

			var addPermissions = request.Add?
				.Where(x => existingClaims.Contains(x, StringComparer.OrdinalIgnoreCase) == false)
				.ToList();

			var removePermissions = request.Remove != null
				? existingClaims.Where(x => request.Remove.Contains(x, StringComparer.OrdinalIgnoreCase))
					.ToList() : null;

			using (var scope = _unitOfWorkFactory.Create())
			{
				if (addPermissions != null)
				{
					foreach (var permission in addPermissions)
					{
						var identityResult = await _roleManager
							.AddClaimAsync(dbRole, new Claim(Permission.ClaimType, permission));

						var result = identityResult.ToApiResult();

						result.AssertSuccess(() =>
							$"Failed to add permission \"{permission}\" to role \"{dbRole.Name}\".");

						_logger.LogInformation("Added permission {permission} to role {role}.", permission,
							dbRole.Name);
					}
				}

				if (removePermissions != null)
				{
					foreach (var permission in removePermissions)
					{
						var identityResult = await _roleManager
							.RemoveClaimAsync(dbRole, new Claim(Permission.ClaimType, permission));

						var result = identityResult.ToApiResult();

						result.AssertSuccess(() => $"Failed to remove permission \"{permission}\" to role \"{dbRole.Name}\".");

						_logger.LogInformation("Removed permission {permission} from role {role}.", permission, dbRole.Name);
					}
				}

				scope.Commit();
			}

			return new ApiResult { AffectedRows = addPermissions?.Count + removePermissions?.Count };
		}
	}
}
