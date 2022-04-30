using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace Host.Services
{
	public static class LoggingHostBuilderExtensions
	{
		public static IHostBuilder UseSerilog(this IHostBuilder builder)
		{
			return builder.UseSerilog((context, _, configuration) =>
				ConfigureLogger(context.Configuration, configuration));
		}

		public static Microsoft.Extensions.Logging.ILogger CreateBootstrapLogger(this WebApplicationBuilder builder)
		{
			var logger = ConfigureLogger(builder.Configuration, new LoggerConfiguration())
				.CreateBootstrapLogger();

			var loggerFactory = new SerilogLoggerFactory(logger);

			return loggerFactory.CreateLogger<Program>();
		}

		private static LoggerConfiguration ConfigureLogger(IConfiguration configuration, LoggerConfiguration logger)
		{
			return logger.ReadFrom.Configuration(configuration);
		}
	}
}
