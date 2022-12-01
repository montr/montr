using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Montr.Idx
{
	public class SignInSettings
	{
		public bool RequireConfirmedEmail { get; set; } = false;

		public bool RequireConfirmedPhoneNumber { get; set; } = false;

		public bool RequireConfirmedAccount { get; set; } = false;

		public void MapTo(SignInOptions options)
		{
			options.RequireConfirmedAccount = RequireConfirmedAccount;
			options.RequireConfirmedEmail = RequireConfirmedEmail;
			options.RequireConfirmedPhoneNumber = RequireConfirmedPhoneNumber;
		}
	}

	public class PasswordSettings
	{
		[Required]
		public int RequiredLength { get; set; } = 6;

		public int RequiredUniqueChars { get; set; } = 1;

		public bool RequireNonAlphanumeric { get; set; } = true;

		public bool RequireLowercase { get; set; } = true;

		public bool RequireUppercase { get; set; } = true;

		public bool RequireDigit { get; set; } = true;

		public void MapTo(PasswordOptions options)
		{
			options.RequiredLength = RequiredLength;
			options.RequiredUniqueChars = RequiredUniqueChars;
			options.RequireNonAlphanumeric = RequireNonAlphanumeric;
			options.RequireLowercase = RequireLowercase;
			options.RequireUppercase =RequireUppercase;
			options.RequireDigit = RequireDigit;
		}
	}

	public class LockoutSettings
	{
		public bool AllowedForNewUsers { get; set; } = true;

		public int MaxFailedAccessAttempts { get; set; } = 5;

		public TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(5);

		public void MapTo(LockoutOptions options)
		{
			options.AllowedForNewUsers = AllowedForNewUsers;
			options.MaxFailedAccessAttempts = MaxFailedAccessAttempts;
			options.DefaultLockoutTimeSpan = DefaultLockoutTimeSpan;
		}
	}
}
