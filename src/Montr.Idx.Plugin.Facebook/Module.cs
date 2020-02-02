using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Idx.Plugin.Facebook
{
	[Module(Dependencies = new [] { typeof(Montr.Idx.Impl.Module) })]
	public class Module : IModule
	{
		private readonly ILogger<Module> _logger;

		public Module(ILogger<Module> logger)
		{
			_logger = logger;
		}

		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			var facebookOptions = configuration.GetOptions<FacebookOptions>();

			if (facebookOptions?.AppId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", FacebookDefaults.AuthenticationScheme);

				services.AddAuthentication().AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
				{
					options.AppId = facebookOptions.AppId;
					options.AppSecret = facebookOptions.AppSecret;
				});
			}
		}
	}
}
