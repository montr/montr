﻿using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Idx.Plugin.Facebook
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
			var facebookOptions = appBuilder.Configuration.GetOptions<FacebookOptions>();

			if (facebookOptions?.AppId != null)
			{
				_logger.LogInformation("Add {scheme} authentication provider", FacebookDefaults.AuthenticationScheme);

				appBuilder.Services.AddAuthentication().AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
				{
					options.AppId = facebookOptions.AppId;
					options.AppSecret = facebookOptions.AppSecret;
				});
			}
		}
	}
}
