using System.Threading;
using Microsoft.AspNetCore.Identity;
using Montr.Core.Services;

namespace Montr.Idx.Impl.Services
{
	public class LocalizedIdentityErrorDescriber : IdentityErrorDescriber
	{
		private readonly ILocalizer _localizer;

		public LocalizedIdentityErrorDescriber(ILocalizer localizer)
		{
			_localizer = localizer;
		}

		public override IdentityError ConcurrencyFailure()
		{
			return Translate(base.ConcurrencyFailure());
		}

		public override IdentityError DefaultError()
		{
			return Translate(base.DefaultError());
		}

		public override IdentityError DuplicateEmail(string email)
		{
			return Translate(base.DuplicateEmail(email), email);
		}

		public override IdentityError DuplicateRoleName(string role)
		{
			return Translate(base.DuplicateRoleName(role), role);
		}

		public override IdentityError DuplicateUserName(string userName)
		{
			return Translate(base.DuplicateUserName(userName), userName);
		}

		public override IdentityError InvalidEmail(string email)
		{
			return Translate(base.InvalidEmail(email), email);
		}

		public override IdentityError InvalidRoleName(string role)
		{
			return Translate(base.InvalidRoleName(role), role);
		}

		public override IdentityError InvalidToken()
		{
			return Translate(base.InvalidToken());
		}

		public override IdentityError InvalidUserName(string userName)
		{
			return Translate(base.InvalidUserName(userName), userName);
		}

		public override IdentityError LoginAlreadyAssociated()
		{
			return Translate(base.LoginAlreadyAssociated());
		}

		public override IdentityError PasswordMismatch()
		{
			return Translate(base.PasswordMismatch());
		}

		public override IdentityError PasswordRequiresDigit()
		{
			return Translate(base.PasswordRequiresDigit());
		}

		public override IdentityError PasswordRequiresLower()
		{
			return Translate(base.PasswordRequiresLower());
		}

		public override IdentityError PasswordRequiresNonAlphanumeric()
		{
			return Translate(base.PasswordRequiresNonAlphanumeric());
		}

		public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
		{
			return Translate(base.PasswordRequiresUniqueChars(uniqueChars), uniqueChars);
		}

		public override IdentityError PasswordRequiresUpper()
		{
			return Translate(base.PasswordRequiresUpper());
		}

		public override IdentityError PasswordTooShort(int length)
		{
			return Translate(base.PasswordTooShort(length), length);
		}

		public override IdentityError RecoveryCodeRedemptionFailed()
		{
			return Translate(base.RecoveryCodeRedemptionFailed());
		}

		public override IdentityError UserAlreadyHasPassword()
		{
			return Translate(base.UserAlreadyHasPassword());
		}

		public override IdentityError UserAlreadyInRole(string role)
		{
			return Translate(base.UserAlreadyInRole(role), role);
		}

		public override IdentityError UserLockoutNotEnabled()
		{
			return Translate(base.UserLockoutNotEnabled());
		}

		public override IdentityError UserNotInRole(string role)
		{
			return Translate(base.UserNotInRole(role), role);
		}

		private IdentityError Translate(IdentityError error, params object[] args)
		{
			var key = $"{nameof(IdentityError)}.{error.Code}";

			var message = _localizer.Get(key, CancellationToken.None).Result;

			if (message != null)
			{
				error.Description = string.Format(message, args);
			}

			return error;
		}
	}
}
