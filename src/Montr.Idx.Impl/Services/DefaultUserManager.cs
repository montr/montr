using System;
using System.Collections.Generic;
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

		public async Task<User> Get(Guid userUid, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var dbUser = await _userManager.FindByIdAsync(userUid.ToString());

			return new()
			{
				Uid = dbUser.Id,
				UserName = dbUser.UserName,
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Email = dbUser.Email,
				PhoneNumber = dbUser.PhoneNumber,
				ConcurrencyStamp = dbUser.ConcurrencyStamp
			};
		}

		public async Task<ApiResult> Create(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

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

		public async Task<ApiResult> Create(User user, string password, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var dbUser = Map(user);

			var identityResult = await _userManager.CreateAsync(dbUser, password);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Created user {userName} with password.", dbUser.UserName);

				result.Uid = dbUser.Id;
				result.ConcurrencyStamp = dbUser.ConcurrencyStamp;
			}

			return result;
		}

		public async Task<ApiResult> Update(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var dbUser = await _userManager.FindByIdAsync(user.Uid.ToString());

			dbUser.ConcurrencyStamp = user.ConcurrencyStamp;

			dbUser.UserName = user.UserName;
			dbUser.LastName = user.LastName;
			dbUser.FirstName = user.FirstName;
			dbUser.Email = user.Email;
			dbUser.PhoneNumber = user.PhoneNumber;

			var identityResult =  await _userManager.UpdateAsync(dbUser);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Updated user {userName}.", dbUser.UserName);

				result.Uid = dbUser.Id;
				result.ConcurrencyStamp = dbUser.ConcurrencyStamp;
			}

			return result;
		}

		public async Task<ApiResult> Delete(User user, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var dbUser = await _userManager.FindByIdAsync(user.Uid.ToString());

			dbUser.ConcurrencyStamp = user.ConcurrencyStamp;

			var identityResult =  await _userManager.DeleteAsync(dbUser);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Deleted user {userName}.", dbUser.UserName);

				result.Uid = dbUser.Id;
			}

			return result;
		}

		public async Task<ApiResult> AddRoles(Guid userUid, IList<string> roles, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var dbUser = await _userManager.FindByIdAsync(userUid.ToString());

			var identityResult =  await _userManager.AddToRolesAsync(dbUser, roles);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Added user {userName} to roles {roles}.", dbUser.UserName, roles);

				result.Uid = dbUser.Id;
			}

			return result;
		}

		public async Task<ApiResult> RemoveRoles(Guid userUid, IList<string> roles, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var dbUser = await _userManager.FindByIdAsync(userUid.ToString());

			var identityResult =  await _userManager.RemoveFromRolesAsync(dbUser, roles);

			var result = identityResult.ToApiResult();

			if (result.Success)
			{
				_logger.LogInformation("Removed user {userName} from roles {roles}.", dbUser.UserName, roles);

				result.Uid = dbUser.Id;
			}

			return result;
		}

		private static DbUser Map(User user)
		{
			return new()
			{
				Id = user.Uid ?? throw new ArgumentException("Id of created user can not be empty as it referencing classifier.", nameof(user)),
				UserName = user.UserName,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				PhoneNumber = user.PhoneNumber
			};
		}
	}
}
