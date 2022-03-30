using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Host.Services
{
	public static class LoggingHostBuilderExtensions
	{
		public static IHostBuilder UseLogging(this IHostBuilder builder)
		{
			return builder.UseSerilog((context, configuration) => ConfigureLogger(context.HostingEnvironment, configuration));
		}

		/*public static IWebHostBuilder UseLogging(this IWebHostBuilder builder)
		{
			return builder.UseSerilog((context, configuration) => ConfigureLogger(context.HostingEnvironment, configuration));
		}*/

		private static void ConfigureLogger(IHostEnvironment environment, LoggerConfiguration configuration)
		{
			configuration/*.MinimumLevel.Debug()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.MinimumLevel.Override("System", LogEventLevel.Warning)
				.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
				.MinimumLevel.Override("Hangfire.Processing.BackgroundExecution", LogEventLevel.Information)
				.MinimumLevel.Override("Hangfire.Server.ServerHeartbeatProcess", LogEventLevel.Information)
				.MinimumLevel.Override("Sentry.ISentryClient", LogEventLevel.Information)*/
				.Enrich.FromLogContext()
				.WriteTo.File($"../../../.logs/{typeof(Startup).Namespace}-{environment.EnvironmentName}.log")
				.WriteTo.Console(outputTemplate: "{Timestamp:o} [{Level:w4}] {SourceContext} - {Message:lj}{NewLine}{Exception}");
		}
	}
}
