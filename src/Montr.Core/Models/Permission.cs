using System;

namespace Montr.Core.Models
{
	public class Permission
	{
		public static readonly string TypeCode = nameof(Permission).ToLower();

		public const string ClaimType = nameof(Permission);

		public string Name { get; }

		public static string GetName(Type permission)
		{
			return permission.FullName;
		}

		public Permission(Type permission) : this(GetName(permission))
		{
		}

		public Permission(string name)
		{
			Name = name;
		}
	}
}
