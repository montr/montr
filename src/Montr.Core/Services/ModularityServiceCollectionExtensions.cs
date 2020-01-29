using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Montr.Core.Services
{
	public static class ModularityServiceCollectionExtensions
	{
		public static ICollection<IModule> AddModules(this IServiceCollection services, IConfiguration configuration, ILogger logger)
		{
			var modules = GetSortedModules(logger);

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
					logger.LogInformation("Initializing module {module}", module);
				}

				// todo: configure module services later (?)
				module.ConfigureServices(configuration, services);

				result.Add(module);
			}

			return result.AsReadOnly();
		}

		private static IList<Type> GetSortedModules(ILogger logger)
		{
			PreloadAssemblies(logger);

			var modules = new List<ModuleInfo>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(x => x.IsClass && x.IsAbstract == false && typeof(IModule).IsAssignableFrom(x)))
				{
					modules.Add(new ModuleInfo
					{
						Type = type,
						Dependencies = type.GetCustomAttribute<ModuleAttribute>()?.Dependencies
					});
				}
			}

			var assemblyModules = modules.ToLookup(x => x.Type.Assembly.GetName().Name);

			// if manual dependencies not specified - resolve dependencies dynamically from referenced assemplies
			foreach (var module in modules)
			{
				if (module.Dependencies == null)
				{
					var dependencies = new List<Type>();

					foreach (var assembly in module.Type.Assembly.GetReferencedAssemblies())
					{
						foreach (var dependency in assemblyModules[assembly.Name])
						{
							dependencies.Add(dependency.Type);
						}
					}

					module.Dependencies = dependencies.ToArray();
				}
			}

			var sortedModules = DirectedAcyclicGraphVerifier.TopologicalSort(modules, node => node.Type, node => node.Dependencies);

			if (logger.IsEnabled(LogLevel.Debug))
			{
				logger.LogDebug("Modules initialization order:");

				foreach (var module in sortedModules)
				{
					if (module.Dependencies.Length == 0)
					{
						logger.LogDebug("· {module}", module.Type);
					}
					else
					{
						logger.LogDebug("· {module} (depends on: {deps})", module.Type, module.Dependencies);
					}
				}
			}

			return sortedModules.Select(x => x.Type).ToArray();
		}

		private static void PreloadAssemblies(ILogger logger)
		{
			var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.IsDynamic == false) // exclude dynamic assemblies without location
				.ToArray();

			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			if (logger.IsEnabled(LogLevel.Information))
			{
				logger.LogInformation("Preloading assemblies from {directory}", baseDirectory);
			}

			foreach (var file in Directory.EnumerateFiles(baseDirectory, "*.dll"))
			{
				if (allAssemblies.FirstOrDefault(
						x => string.Equals(x.Location, file, StringComparison.OrdinalIgnoreCase)) == null)
				{
					if (logger.IsEnabled(LogLevel.Debug))
					{
						logger.LogDebug("· {file}", file.Replace(baseDirectory, "./"));
					}

					Assembly.LoadFrom(file);
				}
			}
		}

		public class ModuleInfo
		{
			public Type Type { get; set; }

			public Type[] Dependencies { get; set; }
		}
	}
}
