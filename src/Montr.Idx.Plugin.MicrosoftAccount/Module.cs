using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Idx.Plugin.MicrosoftAccount
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
			var microsoftOptions = configuration.GetOptions<MicrosoftAccountOptions>();

			if (microsoftOptions?.ClientId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", MicrosoftAccountDefaults.AuthenticationScheme);

				services.AddAuthentication().AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, options =>
				{
					options.ClientId = microsoftOptions.ClientId;
					options.ClientSecret = microsoftOptions.ClientSecret;
				});
			}
		}
	}
}
