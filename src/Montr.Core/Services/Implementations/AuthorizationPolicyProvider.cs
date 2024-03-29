﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Montr.Core.Models;

namespace Montr.Core.Services.Implementations
{
	public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
	{
		private readonly AuthorizationOptions _options;

		public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
		{
			_options = options.Value;
		}

		public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
		{
			var policy = await base.GetPolicyAsync(policyName);

			if (policy == null && PermissionPolicy.TryGetPermission(policyName, out var permission))
			{
				policy = new AuthorizationPolicyBuilder()
					.AddRequirements(new PermissionRequirement(permission))
					.Build();

				_options.AddPolicy(policyName, policy);
			}

			return policy;
		}
	}
}
