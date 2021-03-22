using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.Services
{
	public class DefaultRoleManager : IRoleManager
	{
		private readonly ILogger<DefaultRoleManager> _logger;
		private readonly RoleManager<DbRole> _roleManager;

		public DefaultRoleManager(ILogger<DefaultRoleManager> logger, RoleManager<DbRole> roleManager)
		{
			_logger = logger;
			_roleManager = roleManager;
		}

		public async Task<Role> Get(Guid roleUid, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var dbRole = await _roleManager.FindByIdAsync(roleUid.ToString());

			return new()
			{
				Uid = dbRole.Id,
				Name = dbRole.Name,
				ConcurrencyStamp = dbRole.ConcurrencyStamp
			};
		}

		public async Task<ApiResult> Create(Role role, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var dbRole = new DbRole
			{
				Id = role.Uid ?? throw new ArgumentException("Id of created role can not be empty as it referencing classifier.", nameof(role)),
				Name = role.Name
			};

			var identityResult = await _roleManager.CreateAsync(dbRole);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Created role {name}.", dbRole.Name);

				result.Uid = dbRole.Id;
				result.ConcurrencyStamp = dbRole.ConcurrencyStamp;
			}

			return result;
		}

		public async Task<ApiResult> Update(Role role, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var roleId = role.Uid.ToString();

			var dbRole = await _roleManager.FindByIdAsync(roleId);

			dbRole.ConcurrencyStamp = role.ConcurrencyStamp;

			dbRole.Name = role.Name;

			var identityResult =  await _roleManager.UpdateAsync(dbRole);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Updated role {name}.", dbRole.Name);

				result.Uid = dbRole.Id;
				result.ConcurrencyStamp = dbRole.ConcurrencyStamp;
			}

			return result;
		}

		public async Task<ApiResult> Delete(Role role, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var roleId = role.Uid.ToString();

			var dbRole = await _roleManager.FindByIdAsync(roleId);

			dbRole.ConcurrencyStamp = role.ConcurrencyStamp;

			var identityResult =  await _roleManager.DeleteAsync(dbRole);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Deleted role {name}.", dbRole.Name);

				result.Uid = dbRole.Id;
			}

			return result;
		}
	}
}
