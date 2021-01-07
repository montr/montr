using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Models;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.Services
{
	public class DefaultUserManager : IUserManager
	{
		private readonly UserManager<DbUser> _userManager;

		public DefaultUserManager(UserManager<DbUser> userManager)
		{
			_userManager = userManager;
		}

		public async Task<ApiResult> CreateUser(User user, string optionalPassword, CancellationToken cancellationToken)
		{
			var userUid = user.Uid ?? Guid.NewGuid();

			var dbUser = new DbUser
			{
				Id = userUid,
				UserName = user.UserName,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
			};

			var identityResult = optionalPassword == null
				? await _userManager.CreateAsync(dbUser)
				: await _userManager.CreateAsync(dbUser, optionalPassword);

			var result = identityResult.ToApiResult();

			if (result.Success) result.Uid = userUid;

			return result;
		}
	}
}
