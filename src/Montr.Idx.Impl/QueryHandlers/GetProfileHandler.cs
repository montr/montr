using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Idx.Queries;

namespace Montr.Idx.Impl.QueryHandlers
{
	public class GetProfileHandler : IRequestHandler<GetProfile, ProfileModel>
	{
		private readonly UserManager<DbUser> _userManager;

		public GetProfileHandler(
			UserManager<DbUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<ProfileModel> Handle(GetProfile request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);

			if (user == null)
			{
				return null;
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			return new ProfileModel
			{
				UserName = await _userManager.GetUserNameAsync(user),
				PhoneNumber = await _userManager.GetPhoneNumberAsync(user),
				HasPassword = await _userManager.HasPasswordAsync(user),
				IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
				IsPhoneNumberConfirmed = await _userManager.IsPhoneNumberConfirmedAsync(user)
			};
		}
	}
}
