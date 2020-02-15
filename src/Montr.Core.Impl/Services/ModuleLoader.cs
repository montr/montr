﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class ModuleLoader
	{
		private readonly ILogger _logger;

		public ModuleLoader(ILogger logger)
		{
			_logger = logger;
		}

		public IList<Type> GetSortedModules(string baseDirectory)
		{
			PreloadAssemblies(baseDirectory);

			var modules = new List<ModuleInfo>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes()
					.Where(x => x.IsClass && x.IsAbstract == false && typeof(IModule).IsAssignableFrom(x)))
				{
					var moduleAttribute = type.GetCustomAttribute<ModuleAttribute>();

					modules.Add(new ModuleInfo
					{
						Type = type,
						DependsOn = moduleAttribute?.DependsOn
					});
				}
			}

			var assemblyModules = modules.ToLookup(x => x.Type.Assembly.GetName().Name);

			// if manual dependencies not specified - resolve dependencies dynamically from referenced assemblies
			foreach (var module in modules)
			{
				if (module.DependsOn == null)
				{
					var dependencies = new List<Type>();

					foreach (var assembly in module.Type.Assembly.GetReferencedAssemblies())
					{
						foreach (var dependency in assemblyModules[assembly.Name])
						{
							dependencies.Add(dependency.Type);
						}
					}

					module.DependsOn = dependencies.ToArray();
				}
			}

			var sortedModules = DirectedAcyclicGraphVerifier.TopologicalSort(modules, node => node.Type, node => node.DependsOn);

			if (_logger.IsEnabled(LogLevel.Information))
			{
				_logger.LogInformation("Modules initialization order:");

				foreach (var module in sortedModules)
				{
					if (module.DependsOn.Length == 0)
					{
						_logger.LogInformation("· {module}", module.Type);
					}
					else
					{
						_logger.LogInformation("· {module} (depends on: {deps})", module.Type, module.DependsOn);
					}
				}
			}

			return sortedModules.Select(x => x.Type).ToArray();
		}

		public void PreloadAssemblies(string baseDirectory)
		{
			var allAssemblies = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.IsDynamic == false) // exclude dynamic assemblies without location
				.ToArray();

			if (_logger.IsEnabled(LogLevel.Information))
			{
				_logger.LogInformation("Preloading assemblies from {directory}", baseDirectory);
			}

			foreach (var file in Directory.EnumerateFiles(baseDirectory, "*.dll"))
			{
				if (_logger.IsEnabled(LogLevel.Information))
				{
					_logger.LogInformation("--- {file} {fullPath}", file.Replace(baseDirectory, "./"), file);
				}
			}

			foreach (var file in Directory.EnumerateFiles(baseDirectory, "*.dll"))
			{
				if (allAssemblies.FirstOrDefault(
						x => string.Equals(x.Location, file, StringComparison.OrdinalIgnoreCase)) == null)
				{
					if (_logger.IsEnabled(LogLevel.Information))
					{
						_logger.LogInformation("· {file}", file.Replace(baseDirectory, "./"));
					}

					Assembly.LoadFrom(file);
				}
			}
		}

		public class ModuleInfo
		{
			public Type Type { get; set; }

			public Type[] DependsOn { get; set; }
		}
	}
}
