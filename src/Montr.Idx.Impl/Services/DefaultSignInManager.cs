using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.Services
{
	public class DefaultSignInManager : ISignInManager
	{
		private readonly SignInManager<DbUser> _signInManager;

		public DefaultSignInManager(SignInManager<DbUser> signInManager)
		{
			_signInManager = signInManager;
		}

		public async Task<ApiResult> PasswordSignIn(string userName, string password, bool isPersistent, bool lockoutOnFailure)
		{
			var result = await _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

			// todo: move all result checks from LoginHandler here

			return new ApiResult
			{
				Success = result.Succeeded
			};
		}
	}
}
