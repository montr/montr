using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Montr.Core.Services
{
	public static class HostBuilderExtensions
	{
		public static IHostBuilder UseDefaultConfiguration(this IHostBuilder hostBuilder, string[] args)
		{
			return hostBuilder.ConfigureAppConfiguration((context, config) =>
			{
				var env = context.HostingEnvironment;

				config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
					.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
					.AddUserSecrets(Assembly.Load(new AssemblyName(env.ApplicationName)), optional: true) // todo: remove
					.AddEnvironmentVariables()
					.AddCommandLine(args);
			});
		}
	}
}
