using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Settings.Models;
using Montr.Settings.Services;

namespace Montr.Idx.Auth.Facebook
{
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

	public class FacebookAuthOptions : OAuthOptions
	{
	}

	public class ConfigurationStartupTask : IStartupTask
	{
		private readonly ISettingsTypeRegistry _settingsTypeRegistry;
		private readonly IConfigurationRegistry _registry;

		public ConfigurationStartupTask(ISettingsTypeRegistry settingsTypeRegistry, IConfigurationRegistry registry)
		{
			_settingsTypeRegistry = settingsTypeRegistry;
			_registry = registry;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_settingsTypeRegistry.Register(typeof(FacebookAuthOptions));

			_registry.Configure<Application>(config =>
			{
				config.Add<SettingsPane>((_, settings) =>
				{
					settings.Type = typeof(FacebookAuthOptions);
					settings.Category = SettingsCategory.OAuthProviders;
				});
			});

			return Task.CompletedTask;
		}
	}
}
