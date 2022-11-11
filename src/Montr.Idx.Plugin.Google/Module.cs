using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Idx.Plugin.Google
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
			var googleOptions = appBuilder.Configuration.GetOptions<GoogleOptions>();

			if (googleOptions?.ClientId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", GoogleDefaults.AuthenticationScheme);

				appBuilder.Services.AddAuthentication().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
				{
					options.ClientId = googleOptions.ClientId;
					options.ClientSecret = googleOptions.ClientSecret;
				});
			}
		}
	}
}
