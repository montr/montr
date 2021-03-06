﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class ModuleLoader
	{
		private readonly ILogger _logger;

		private readonly Regex _excludePattern = new(@".*(Microsoft\.|System\.).*");

		public IList<Exception> Errors { get; } = new List<Exception>();

		public ModuleLoader(ILogger logger)
		{
			_logger = logger;
		}

		public IList<Type> GetSortedModules(string baseDirectory, bool throwOnErrors)
		{
			Errors.Clear();

			PreloadAssemblies(baseDirectory);

			var modules = new List<ModuleInfo>();

			foreach (var assembly in GetAssemblies())
			{
				try
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
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to read modules from {assembly}", assembly);

					Errors.Add(ex);
				}
			}

			if (throwOnErrors && Errors.Count > 0)
			{
				throw new ApplicationException($"One or more errors ({Errors.Count}) occured while loading modules, see logs for details.");
			}

			var assemblyModules = modules.ToLookup(x => x.Type.Assembly.GetName().Name);

			// if manual dependencies with [Module] attribute is not specified - resolve dependencies dynamically from referenced assemblies
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
				_logger.LogInformation("Modules initialization order ({count}):", sortedModules.Count);

				foreach (var module in sortedModules)
				{
					if (module.DependsOn.Length == 0)
					{
						_logger.LogInformation("• {module}", module.Type);
					}
					else
					{
						_logger.LogInformation("• {module} (depends on: {deps})", module.Type, module.DependsOn);
					}
				}
			}

			return sortedModules.Select(x => x.Type).ToArray();
		}

		public void PreloadAssemblies(string baseDirectory)
		{
			if (_logger.IsEnabled(LogLevel.Information))
			{
				_logger.LogInformation("Preloading assemblies from {directory}", baseDirectory);
			}

			var allAssemblies = GetAssemblies().ToDictionary(x => x.Location);

			foreach (var file in Directory.EnumerateFiles(baseDirectory, "*.dll"))
			{
				if (ExcludeAssembly(file)) continue;

				if (allAssemblies.TryGetValue(file, out _) == false)
				{
					if (_logger.IsEnabled(LogLevel.Debug))
					{
						_logger.LogDebug("• {file}", file.Replace(baseDirectory, string.Empty));
					}

					try
					{
						Assembly.LoadFrom(file);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "x Failed to preload assembly from {file}", file);

						Errors.Add(ex);
					}
				}
			}
		}

		public IEnumerable<Assembly> GetAssemblies()
		{
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.IsDynamic == false) // exclude dynamic assemblies without location
				.Where(x => ExcludeAssembly(x.Location) == false);
		}

		public bool ExcludeAssembly(string file)
		{
			return _excludePattern.Match(file).Success;
		}

		public class ModuleInfo
		{
			public Type Type { get; set; }

			public Type[] DependsOn { get; set; }
		}
	}
}
