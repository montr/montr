using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Montr.Core.Services;

namespace Montr.Idx.Services.Implementations
{
	public class IdentityOptionsConfigurator : IConfigureOptions<IdentityOptions>
	{
		private readonly IConfiguration _configuration;

		public IdentityOptionsConfigurator(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void Configure(IdentityOptions options)
		{
			_configuration.GetOptions<SignInSettings>().MapTo(options.SignIn);
			_configuration.GetOptions<PasswordSettings>().MapTo(options.Password);
			_configuration.GetOptions<LockoutSettings>().MapTo(options.Lockout);
		}
	}
}
