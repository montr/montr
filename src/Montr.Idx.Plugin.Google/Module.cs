using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Idx.Plugin.Google
{
	[Module(Dependencies = new[] { typeof(Montr.Idx.Impl.Module) })]
	public class Module : IModule
	{
		private readonly ILogger<Module> _logger;

		public Module(ILogger<Module> logger)
		{
			_logger = logger;
		}

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			var googleOptions = configuration.GetOptions<GoogleOptions>();

			if (googleOptions?.ClientId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", GoogleDefaults.AuthenticationScheme);

				services.AddAuthentication().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
				{
					options.ClientId = googleOptions.ClientId;
					options.ClientSecret = googleOptions.ClientSecret;
				});
			}
		}
	}
}
