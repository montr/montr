using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Montr.Core.Models;

namespace Montr.Core.Services.Implementations
{
	public class DefaultConfigurationProvider : IConfigurationProvider
	{
		private readonly IConfigurationRegistry _configurationRegistry;
		private readonly IAuthorizationService _authorizationService;

		public DefaultConfigurationProvider(IConfigurationRegistry configurationRegistry, IAuthorizationService authorizationService)
		{
			_configurationRegistry = configurationRegistry;
			_authorizationService = authorizationService;
		}

		public async Task<ICollection<T>> GetItems<TEntity, T>(TEntity entity, ClaimsPrincipal principal) where T : IConfigurationItem, new()
		{
			var result = new List<T>();

			// todo: check authorization before instantiating items
			foreach (var item in _configurationRegistry.GetItems<TEntity, T>(entity))
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
