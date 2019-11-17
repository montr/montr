using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Queries;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetExternalLoginsHandler : IRequestHandler<GetExternalLogins, IList<UserLoginInfo>>
	{
		private readonly UserManager<DbUser> _userManager;

		public GetExternalLoginsHandler(UserManager<DbUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<IList<UserLoginInfo>> Handle(GetExternalLogins request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);
			if (user == null)
			{
				return null;
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			return await _userManager.GetLoginsAsync(user);

			/*OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
				.Where(auth => CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
				.ToList();
			ShowRemoveButton = user.PasswordHash != null || CurrentLogins.Count > 1;*/
		}
	}
}
