using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Entities;
using Montr.Idx.Models;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.Idx.Services.Implementations
{
	public class RegisterPermissionsStartupTask : IStartupTask
	{
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly IEnumerable<IPermissionProvider> _permissionProviders;
		private readonly RoleManager<DbRole> _roleManager;

		public RegisterPermissionsStartupTask(
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			IEnumerable<IPermissionProvider> permissionProviders,
			RoleManager<DbRole> roleManager)
		{
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_permissionProviders = permissionProviders;
			_roleManager = roleManager;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			await InsertPermissions(cancellationToken);

			await InsertRoleTemplates(cancellationToken);
		}

		private async Task InsertPermissions(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var repository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.Permission);

			var existingPermissions =
				(await repository.Search(new ClassifierSearchRequest
				{
					TypeCode = ClassifierTypeCode.Permission,
					SkipPaging = true
				}, cancellationToken)).Rows.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

			var newPermissions = new List<Permission>();

			foreach (var provider in _permissionProviders)
			{
				var permissions = provider.GetPermissions();

				foreach (var permission in permissions)
				{
					if (existingPermissions.ContainsKey(permission.Code) == false)
					{
						newPermissions.Add(permission);
					}
				}
			}

			// todo: use bulk insert
			foreach (var permission in newPermissions)
			{
				await repository.Insert(new Classifier
				{
					Type = ClassifierTypeCode.Permission,
					Code = permission.Code,
					IsSystem = true,
					StatusCode = ClassifierStatusCode.Active
				}, cancellationToken);
			}
		}

		private async Task InsertRoleTemplates(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var repository = _classifierRepositoryFactory.GetNamedOrDefaultService(ClassifierTypeCode.Role);

			foreach (var provider in _permissionProviders)
			{
				foreach (var roleTemplate in provider.GetRoleTemplates())
				{
					var dbRole = await _roleManager.FindByNameAsync(roleTemplate.RoleCode);

					if (dbRole == null)
					{
						var result = await repository.Insert(new Role
						{
							Type = ClassifierTypeCode.Role,
							Code = roleTemplate.RoleCode,
							Name = roleTemplate.RoleCode,
							IsSystem = true,
							StatusCode = ClassifierStatusCode.Active
						}, cancellationToken);

						result.AssertSuccess(() => $"Failed to register role template {roleTemplate.RoleCode}");

						dbRole = await _roleManager.FindByNameAsync(roleTemplate.RoleCode);
					}

					// todo: merge with ManageRolePermissionsHandler
					var existingClaims = (await _roleManager.GetClaimsAsync(dbRole))
						.Where(x => x.Type == Permission.ClaimType)
						.Select(x => x.Value);

					var addPermissions = roleTemplate.Permissions?
						.Select(x => x.Code)
						.Where(x => existingClaims.Contains(x, StringComparer.OrdinalIgnoreCase) == false)
						.ToList();

					if (addPermissions != null)
					{
						foreach (var permission in addPermissions)
						{
							var identityResult = await _roleManager
								.AddClaimAsync(dbRole, new Claim(Permission.ClaimType, permission));

							var result = identityResult.ToApiResult();

							result.AssertSuccess(() =>
								$"Failed to add permission \"{permission}\" to role \"{dbRole.Name}\".");
						}
					}
				}
			}
		}
	}
}
