using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.QueryHandlers
{
	public class GetMenuHandler : IRequestHandler<GetMenu, Menu>
	{
		private readonly IAuthorizationService _authorizationService;
		private readonly IContentService _contentService;

		public GetMenuHandler(IAuthorizationService authorizationService, IContentService contentService)
		{
			_authorizationService = authorizationService;
			_contentService = contentService;
		}

		public async  Task<Menu> Handle(GetMenu request, CancellationToken cancellationToken)
		{
			var menu = _contentService.GetMenu(request.MenuId);

			var result = await GetAuthorizedItems(menu?.Items, request.Principal);

			return new Menu { Id = request.MenuId, Items = result };
		}

		private async Task<IList<Menu>> GetAuthorizedItems(ICollection<Menu> items, ClaimsPrincipal principal)
		{
			if (items?.Count > 0)
			{
				var result = new List<Menu>();

				foreach (var item in items)
				{
					if (await Authorize(item, principal) != false)
					{
						var children = item.Items;
						var authorizedChildren = await GetAuthorizedItems(children, principal);

						// if exists any children and all children not authorized - hide parent menu item
						if (children?.Count > 0 && authorizedChildren?.Count == 0) continue;

						var authorizedItem = item.Clone();
						authorizedItem.Items = authorizedChildren;
						result.Add(authorizedItem);
					}
				}

				return result;
			}

			return null;
		}

		private async Task<bool?> Authorize(Menu item, ClaimsPrincipal principal)
		{
			if (item?.Permission == null) return null;

			var authResult = await _authorizationService.AuthorizePermission(principal, item.Permission);

			return authResult?.Succeeded;
		}
	}
}
