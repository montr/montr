using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Services;
using Serilog;
using Serilog.Events;

namespace Host
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var hostBuilder = WebHost
				.CreateDefaultBuilder(args)
				/*.ConfigureAppConfiguration((builderContext, config) =>
				{
					var env = builderContext.HostingEnvironment;

					config.SetBasePath(Directory.GetCurrentDirectory());
					config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
					config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
					config.AddEnvironmentVariables();
				})*/
				.UseStartup<Startup>()
				.UseSentry()
				.UseSerilog((context, configuration) =>
				{
					configuration
						.MinimumLevel.Debug()
						.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
						.MinimumLevel.Override("System", LogEventLevel.Warning)
						.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
						.MinimumLevel.Override("Hangfire.Processing.BackgroundExecution", LogEventLevel.Information)
						.MinimumLevel.Override("Hangfire.Server.ServerHeartbeatProcess", LogEventLevel.Information)
						.Enrich.FromLogContext()
						.WriteTo.File($"../../../.logs/{typeof(Startup).Namespace}-{context.HostingEnvironment.EnvironmentName}.log")
						.WriteTo.Console(outputTemplate: "{Timestamp:o} [{Level:w4}] {SourceContext} - {Message:lj}{NewLine}{Exception}");
				});

			var host = hostBuilder.Build();

			using (var scope = host.Services.CreateScope())
			{
				var serviceProvider = scope.ServiceProvider;

				var cancellationToken = CancellationToken.None;

				await serviceProvider.GetService<IMigrationRunner>().Run(cancellationToken);

				foreach (var startupTask in serviceProvider.GetServices<IStartupTask>())
				{
					await startupTask.Run(cancellationToken);
				}
			}

			await host.RunAsync();
		}
	}
}
