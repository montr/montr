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
	public class AddRolePermissionsHandler : IRequestHandler<AddRolePermissions, ApiResult>
	{
		private readonly ILogger<AddRolePermissionsHandler> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly RoleManager<DbRole> _roleManager;

		public AddRolePermissionsHandler(ILogger<AddRolePermissionsHandler> logger,
			IUnitOfWorkFactory unitOfWorkFactory, RoleManager<DbRole> roleManager)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_roleManager = roleManager;
		}

		public async Task<ApiResult> Handle(AddRolePermissions request, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var roleUid = request?.RoleUid ?? throw new ArgumentNullException(nameof(request));

			var dbRole = await _roleManager.FindByIdAsync(roleUid.ToString());

			var existingClaims = (await _roleManager.GetClaimsAsync(dbRole))
				.Where(x => x.Type == Permission.ClaimType)
				.Select(x => x.Value);

			var permissions = request.Permissions
				.Where(x => existingClaims.Contains(x, StringComparer.OrdinalIgnoreCase) == false)
				.ToList();

			using (var scope = _unitOfWorkFactory.Create())
			{
				foreach (var permission in permissions)
				{
					var identityResult = await _roleManager
						.AddClaimAsync(dbRole, new Claim(Permission.ClaimType, permission));

					var result = identityResult.ToApiResult();

					result.AssertSuccess(() => $"Failed to add permission \"{permission}\" to role \"{dbRole.Name}\".");

					_logger.LogInformation("Added permission {permission} to role {role}.", permission, dbRole.Name);
				}

				scope.Commit();
			}

			return new ApiResult { AffectedRows = permissions.Count };
		}
	}

	public class RemoveRolePermissionsHandler : IRequestHandler<RemoveRolePermissions, ApiResult>
	{
		private readonly ILogger<RemoveRolePermissionsHandler> _logger;

		public RemoveRolePermissionsHandler(ILogger<RemoveRolePermissionsHandler> logger)
		{
			_logger = logger;
		}

		public Task<ApiResult> Handle(RemoveRolePermissions request, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}
	}
}
