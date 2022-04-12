using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public static class ModularServiceExtensions
	{
		public static ICollection<IModule> AddModules(this IServiceCollection services, IConfiguration configuration, ILogger logger)
		{
			var loader = new ModuleLoader(logger);

			var sortedModuleTypes = loader.GetSortedModules(AppDomain.CurrentDomain.BaseDirectory, true);

			// register module types to create modules later with dependencies
			foreach (var type in sortedModuleTypes)
			{
				// modules instances will be created twice (?) (with temp and real service providers)
				// todo: create modules using ActivatorUtilities?
				services.AddTransient(type);
			}

			// temp service provider to create modules
			IServiceProvider serviceProvider = services.BuildServiceProvider();

			var result = new List<IModule>();

			foreach (var type in sortedModuleTypes)
			{
				var module = (IModule)serviceProvider.GetRequiredService(type);

				// to prevent modules to be created twice
				services.AddTransient(typeof(IModule), _ => module);

				if (logger.IsEnabled(LogLevel.Information))
				{
					logger.LogInformation("Initializing {module}", module);
				}

				// todo: configure module services later (?)
				// module.ConfigureServices(configuration, services);

				result.Add(module);
			}

			return result.AsReadOnly();
		}

		public static async Task RunStartupTasks(this IServiceProvider services, ILogger logger, CancellationToken cancellationToken = default)
		{
			using (var scope = services.CreateScope())
			{
				// todo: IStartupTask of modules should be run right after modules -
				// - so either modules or startup tasks should left, remove other
				foreach (var module in scope.ServiceProvider.GetServices<IModule>())
				{
					if (module is IStartupTask startupTask)
					{
						logger.LogInformation("Running startup module {module}", module);

						await startupTask.Run(cancellationToken);
					}
				}

				// todo: run startup tasks from modules or sort IStartupTask's by module initialization order
				// fixme: startup tasks already ordered, because they are registered in ordered modules
				foreach (var task in scope.ServiceProvider.GetServices<IStartupTask>())
				{
					logger.LogInformation("Running startup task {task}", task);

					await task.Run(cancellationToken);
				}
			}
		}
	}
}
