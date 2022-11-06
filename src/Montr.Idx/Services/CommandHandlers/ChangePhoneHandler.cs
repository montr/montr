using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Idx.Commands;
using Montr.Idx.Entities;
using Montr.Idx.Services.Implementations;

namespace Montr.Idx.Services.CommandHandlers
{
	public class ChangePhoneHandler : IRequestHandler<ChangePhone, ApiResult>
	{
		private readonly UserManager<DbUser> _userManager;
		private readonly SignInManager<DbUser> _signInManager;

		public ChangePhoneHandler(
			UserManager<DbUser> userManager,
			SignInManager<DbUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public async Task<ApiResult> Handle(ChangePhone request, CancellationToken cancellationToken)
		{
			var user = await _userManager.GetUserAsync(request.User);
			if (user == null)
			{
				return new ApiResult { Success = false };
				// return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

			if (request.PhoneNumber != phoneNumber)
			{
				var identityResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);

				if (identityResult.Succeeded == false)
				{
					return identityResult.ToApiResult();
				}
			}

			await _signInManager.RefreshSignInAsync(user);

			return new ApiResult();
		}
	}
}
