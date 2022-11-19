using System;
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

		public async Task<ICollection<TItem>> GetItems<TItem>(Type ofEntity, object entity, ClaimsPrincipal principal) where TItem : IConfigurationItem, new()
		{
			var result = new List<TItem>();

			// todo: check authorization before instantiating items
			foreach (var item in _configurationRegistry.GetItems<TItem>(ofEntity, entity))
			{
				if (await Authorize(item, principal) != false)
				{
					result.Add(item);
				}
			}

			return result.OrderBy(x => x.DisplayOrder).ToImmutableList();
		}

		public async Task<ICollection<TItem>> GetItems<TEntity, TItem>(TEntity entity, ClaimsPrincipal principal) where TItem : IConfigurationItem, new()
		{
			return await GetItems<TItem>(typeof(TEntity), entity, principal);
		}

		private async Task<bool?> Authorize(IConfigurationItem item, ClaimsPrincipal principal)
		{
			if (item?.Permission == null) return null;

			var authResult = await _authorizationService.AuthorizePermission(principal, item.Permission);

			return authResult?.Succeeded;
		}
	}
}
