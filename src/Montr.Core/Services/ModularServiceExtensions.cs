using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core.Services.Implementations;

namespace Montr.Core.Services
{
	public static class ModularServiceExtensions
	{
		public static ICollection<IModule> AddModules(this IServiceCollection services, ILogger logger)
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
			var serviceProvider = services.BuildServiceProvider();

			var result = new List<IModule>();

			foreach (var type in sortedModuleTypes)
			{
				var module = (IModule)serviceProvider.GetRequiredService(type);

				// to prevent modules to be created twice
				services.AddTransient(typeof(IModule), _ => module);

				result.Add(module);
			}

			return result.AsReadOnly();
		}

		public static async Task RunStartupTasks(this IApp app, CancellationToken cancellationToken = default)
		{
			using (var scope = app.ApplicationServices.CreateScope())
			{
				foreach (var task in scope.ServiceProvider.GetServices<IStartupTask>())
				{
					app.Logger.LogInformation("Running startup task {task}", task);

					await task.Run(cancellationToken);
				}
			}
		}
	}
}
