using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.Idx.Impl.Services
{
	public class RegisterPrivilegesStartupTask : IStartupTask
	{
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;
		private readonly IEnumerable<IPermissionProvider> _permissionProviders;

		public RegisterPrivilegesStartupTask(
			INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory,
			IEnumerable<IPermissionProvider> permissionProviders)
		{
			_classifierRepositoryFactory = classifierRepositoryFactory;
			_permissionProviders = permissionProviders;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var repository = _classifierRepositoryFactory.GetNamedOrDefaultService(Permission.TypeCode);

			var existingPermissions =
				(await repository.Search(new ClassifierSearchRequest
				{
					TypeCode = Permission.TypeCode,
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
					Type = Permission.TypeCode,
					Code = permission.Code,
					IsSystem = true,
					StatusCode = ClassifierStatusCode.Active
				}, cancellationToken);
			}
		}
	}
}
