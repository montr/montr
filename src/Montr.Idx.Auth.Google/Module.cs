using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Services;
using Montr.Idx.Auth.Google.Services;

namespace Montr.Idx.Auth.Google
{
	[Module(DependsOn = new[] { typeof(Idx.Module) })]
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		private readonly ILogger<Module> _logger;

		public Module(ILogger<Module> logger)
		{
			_logger = logger;
		}

		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.BindOptions<GoogleAuthOptions>(appBuilder.Configuration);

			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();

			var authOptions = appBuilder.Configuration.GetOptions<GoogleAuthOptions>();

			if (authOptions?.ClientId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", GoogleDefaults.AuthenticationScheme);

				appBuilder.Services.AddAuthentication().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
				{
					options.ClientId = authOptions.ClientId;
					options.ClientSecret = authOptions.ClientSecret;
				});
			}
		}
	}
}
