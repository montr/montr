using System;
using Microsoft.AspNetCore.Authorization;

namespace Montr.Core.Models
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
	public class PermissionAttribute : Attribute, IAuthorizeData
	{
		public string Policy { get; set; }

		public string Roles { get; set; }

		public string AuthenticationSchemes { get; set; }

		public PermissionAttribute(Type permission)
		{
			Policy = PermissionPolicy.BuildPolicyName(permission);
		}
	}
}
