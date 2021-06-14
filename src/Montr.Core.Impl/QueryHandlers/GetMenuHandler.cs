using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Impl.QueryHandlers
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

			var result = await GetAuthorizedItems(menu.Items, request.Principal);

			return new Menu { Items = result };
		}

		private async Task<IList<Menu>> GetAuthorizedItems(IList<Menu> items, ClaimsPrincipal principal)
		{
			if (items != null)
			{
				var result = new List<Menu>();

				foreach (var item in items)
				{
					if (await Authorize(item, principal) != false)
					{
						var authorizedItem = item.Clone();

						result.Add(authorizedItem);

						authorizedItem.Items = await GetAuthorizedItems(item.Items, principal);
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

			return authResult.Succeeded;
		}
	}
}
