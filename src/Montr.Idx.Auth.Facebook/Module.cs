using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Services;
using Montr.Idx.Auth.Facebook.Services;

namespace Montr.Idx.Auth.Facebook
{
	// ReSharper disable once UnusedType.Global
	[Module(DependsOn = new [] { typeof(Idx.Module) })]
	public class Module : IModule
	{
		private readonly ILogger<Module> _logger;

		public Module(ILogger<Module> logger)
		{
			_logger = logger;
		}

		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.BindOptions<FacebookAuthOptions>(appBuilder.Configuration);

			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();

			var authOptions = appBuilder.Configuration.GetOptions<FacebookAuthOptions>();

			if (authOptions?.ClientId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", FacebookDefaults.AuthenticationScheme);

				appBuilder.Services.AddAuthentication().AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
				{
					options.AppId = authOptions.ClientId;
					options.AppSecret = authOptions.ClientSecret;
				});
			}
		}
	}
}
