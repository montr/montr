using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Idx.Entities;
using Montr.Idx.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.Implementations;
using Montr.Metadata.Services;

namespace Montr.Idx.Services.Implementations
{
	public class DbRoleRepository : DbClassifierRepository<Role>
	{
		private readonly ILogger<DbRoleRepository> _logger;
		private readonly RoleManager<DbRole> _roleManager;

		public DbRoleRepository(
			ILogger<DbRoleRepository> logger,
			IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService,
			IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService,
			IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator,
			RoleManager<DbRole> roleManager)
			: base(
				dbContextFactory,
				classifierTypeService,
				classifierTreeService,
				metadataService,
				fieldDataRepository,
				numberGenerator)
		{
			_logger = logger;
			_roleManager = roleManager;
		}

		public override async Task<ApiResult> Insert(Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.Insert(item, cancellationToken);

			if (result.Success)
			{
				var role = (Role) item;

				var dbRole = new DbRole
				{
					Id = role.Uid ?? throw new ArgumentException("Id of created role can not be empty as it referencing classifier.", nameof(role)),
					Name = role.Name
				};

				var identityResult = await _roleManager.CreateAsync(dbRole);

				result = identityResult.ToApiResult();

				if (result.Success)
				{
					_logger.LogInformation("Created role {name}.", dbRole.Name);

					result.Uid = dbRole.Id;
					result.ConcurrencyStamp = dbRole.ConcurrencyStamp;
				}
			}

			return result;
		}

		public override async Task<ApiResult> Update(Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.Update(item, cancellationToken);

			if (result.Success)
			{
				var role = (Role) item;

				var roleId = role.Uid.ToString();

				var dbRole = await _roleManager.FindByIdAsync(roleId);

				// todo: restore optimistic concurrency check (?)
				// dbRole.ConcurrencyStamp = role.ConcurrencyStamp;

				dbRole.Name = role.Name;

				var identityResult =  await _roleManager.UpdateAsync(dbRole);

				result = identityResult.ToApiResult();

				if (result.Success)
				{
					_logger.LogInformation("Updated role {name}.", dbRole.Name);

					result.Uid = dbRole.Id;
					result.ConcurrencyStamp = dbRole.ConcurrencyStamp;
				}
			}

			return result;
		}

		public override async Task<ApiResult> Delete(DeleteClassifier request, CancellationToken cancellationToken)
		{
			foreach (var uid in request.Uids)
			{
				var dbRole = await _roleManager.FindByIdAsync(uid.ToString());

				// todo: restore optimistic concurrency check (?)
				// dbRole.ConcurrencyStamp = role.ConcurrencyStamp;

				var identityResult =  await _roleManager.DeleteAsync(dbRole);

				if (identityResult.Succeeded == false) return identityResult.ToApiResult();

				_logger.LogInformation("Deleted role {name}.", dbRole.Name);
			}

			return await base.Delete(request, cancellationToken);
		}
	}
}
