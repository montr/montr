using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetExternalLoginsHandler : IRequestHandler<GetExternalLogins, IList<ExternalLoginModel>>
	{
		private readonly UserManager<DbUser> _userManager;

		public GetExternalLoginsHandler(UserManager<DbUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IList<ExternalLoginModel>> Handle(GetExternalLogins request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);
			if (user == null)
			{
				return null;
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var result = await _userManager.GetLoginsAsync(user);

			return result.Select(x => new Models.ExternalLoginModel
			{
				LoginProvider = x.LoginProvider,
				ProviderKey = x.ProviderKey,
				ProviderDisplayName = x.ProviderDisplayName
			}).ToList();
		}
	}
}
