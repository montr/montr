using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Idx.Plugin.MicrosoftAccount
{
	[Module(DependsOn = new[] { typeof(Idx.Module) })]
	public class Module : IModule, IAppBuilderConfigurator
	{
		private readonly ILogger<Module> _logger;

		public Module(ILogger<Module> logger)
		{
			_logger = logger;
		}

		public void Configure(IAppBuilder appBuilder)
		{
			var microsoftOptions = appBuilder.Configuration.GetOptions<MicrosoftAccountOptions>();

			if (microsoftOptions?.ClientId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", MicrosoftAccountDefaults.AuthenticationScheme);

				appBuilder.Services.AddAuthentication().AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
				{
					options.ClientId = microsoftOptions.ClientId;
					options.ClientSecret = microsoftOptions.ClientSecret;
				});
			}
		}
	}
}
