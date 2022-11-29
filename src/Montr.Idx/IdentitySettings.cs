using System.ComponentModel;

namespace Montr.Idx
{
	public class IdentitySignInSettings
	{
		[DefaultValue(false)]
		public bool RequireConfirmedEmail { get; set; }

		[DefaultValue(false)]
		public bool RequireConfirmedPhoneNumber { get; set; }

		[DefaultValue(false)]
		public bool RequireConfirmedAccount { get; set; }
	}

	public class IdentityPasswordSettings
	{
		[DefaultValue(6)]
		public int RequiredLength { get; set; }

		[DefaultValue(1)]
		public int RequiredUniqueChars { get; set; }

		[DefaultValue(true)]
		public bool RequireNonAlphanumeric { get; set; }

		[DefaultValue(true)]
		public bool RequireLowercase { get; set; }

		[DefaultValue(true)]
		public bool RequireUppercase { get; set; }

		[DefaultValue(true)]
		public bool RequireDigit { get; set; } = true;
	}
}
