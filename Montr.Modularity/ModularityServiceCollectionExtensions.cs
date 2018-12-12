using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Modularity
{
	public static class ModularityServiceCollectionExtensions
	{
		public static void AddModules(this IServiceCollection services)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in assembly.GetTypes().Where(
					x => x.IsClass && x.IsAbstract == false && typeof(IModuleInitializer).IsAssignableFrom(x)))
				{
					((IModuleInitializer)Activator.CreateInstance(type)).ConfigureServices(services);
				}
			}
		}
	}
}
