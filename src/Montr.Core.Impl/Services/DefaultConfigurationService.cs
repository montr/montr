using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultConfigurationService : IConfigurationService
	{
		private readonly IConfigurationManager _configurationManager;
		private readonly IAuthorizationService _authorizationService;

		public DefaultConfigurationService(IConfigurationManager configurationManager, IAuthorizationService authorizationService)
		{
			_configurationManager = configurationManager;
			_authorizationService = authorizationService;
		}

		public async Task<ICollection<T>> GetItems<TEntity, T>(TEntity entity, ClaimsPrincipal principal) where T : IConfigurationItem
		{
			var result = new List<T>();

			foreach (var item in _configurationManager.GetItems<TEntity, T>(entity))
			{
				if (await Authorize(item, principal) != false)
				{
					result.Add(item);
				}
			}

			return result.OrderBy(x => x.DisplayOrder).ToImmutableList();
		}

		private async Task<bool?> Authorize(IConfigurationItem item, ClaimsPrincipal principal)
		{
			if (item?.Permission == null) return null;

			var authResult = await _authorizationService.AuthorizePermission(principal, item.Permission);

			return authResult?.Succeeded;
		}
	}
}
