using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Idx.Services;

namespace Montr.Idx.Impl.Services
{
	public class DefaultUserManager : IUserManager
	{
		private readonly ILogger<DefaultUserManager> _logger;
		private readonly UserManager<DbUser> _userManager;

		public DefaultUserManager(ILogger<DefaultUserManager> logger, UserManager<DbUser> userManager)
		{
			_logger = logger;
			_userManager = userManager;
		}

		public async Task<User> Get(Guid userUid, CancellationToken cancellationToken)
		{
			var dbUser = await _userManager.FindByIdAsync(userUid.ToString());

			return Map(dbUser);
		}

		public async Task<ApiResult> Create(User user, CancellationToken cancellationToken)
		{
			var dbUser = Map(user);

			var identityResult =  await _userManager.CreateAsync(dbUser);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Created user {userName} without password.", dbUser.UserName);

				result.Uid = dbUser.Id;
			}

			return result;
		}

		public async Task<ApiResult> Create(User user, string password, CancellationToken cancellationToken)
		{
			var dbUser = Map(user);

			var identityResult = await _userManager.CreateAsync(dbUser, password);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Created user {userName} with password.", dbUser.UserName);

				result.Uid = dbUser.Id;
			}

			return result;
		}

		public async Task<ApiResult> Update(User user, CancellationToken cancellationToken)
		{
			var dbUser = Map(user);

			var identityResult =  await _userManager.UpdateAsync(dbUser);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Updated user {userName}.", dbUser.UserName);
			}

			return result;
		}

		public async Task<ApiResult> Delete(User user, CancellationToken cancellationToken)
		{
			var dbUser = Map(user);

			var identityResult =  await _userManager.DeleteAsync(dbUser);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Deleted user {userName}.", dbUser.UserName);
			}

			return result;
		}

		private static DbUser Map(User user)
		{
			return new()
			{
				Id = user.Uid ?? Guid.NewGuid(),
				UserName = user.UserName,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber,
				SecurityStamp = user.SecurityStamp
			};
		}

		private static User Map(DbUser dbUser)
		{
			return new()
			{
				Uid = dbUser.Id,
				UserName = dbUser.UserName,
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Email = dbUser.Email,
				PhoneNumber = dbUser.PhoneNumber,
				SecurityStamp = dbUser.SecurityStamp
			};
		}
	}
}
