using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Montr.Core.Impl.Services
{
	public static class ModularityServiceCollectionExtensions
	{
		public static ICollection<IModule> AddModules(this IServiceCollection services, IConfiguration configuration, ILogger logger)
		{
			var loader = new ModuleLoader(logger);

			var modules = loader.GetSortedModules(AppDomain.CurrentDomain.BaseDirectory, true);

			// register module types to create modules later with dependencies
			foreach (var type in modules)
			{
				// modules instances will be created twice (with temp and real service providers) 
				// todo: create modules using ActivatorUtilities?
				services.AddTransient(type);
			}

			// temp service provider to create modules
			IServiceProvider serviceProvider = services.BuildServiceProvider();

			var result = new List<IModule>();

			foreach (var type in modules)
			{
				var module = (IModule)serviceProvider.GetService(type);

				if (logger.IsEnabled(LogLevel.Information))
				{
					logger.LogInformation("Initializing {module}", module);
				}

				// todo: configure module services later (?)
				module.ConfigureServices(configuration, services);

				result.Add(module);
			}

			return result.AsReadOnly();
		}
	}
}
