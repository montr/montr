namespace Montr.Core.Models
{
	public class Permission
	{
		public const string ClaimType = nameof(Permission);

		public const string PolicyPrefix = nameof(Permission) + ":";

		public string Name { get; }

		public Permission(string name)
		{
			Name = name;
		}
	}
}
