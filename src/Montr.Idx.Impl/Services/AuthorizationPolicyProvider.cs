using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Impl.Services
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

			if (policy == null && policyName.StartsWith(Permission.PolicyPrefix))
			{
				var permission = policyName[Permission.PolicyPrefix.Length..];

				policy = new AuthorizationPolicyBuilder()
					.AddRequirements(new PermissionRequirement(permission))
					.Build();

				_options.AddPolicy(policyName, policy);
			}

			return policy;
		}
	}
}
