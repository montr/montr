using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Impl.Services;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Serilog;
using Serilog.Events;

namespace Host
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await Migrate(args);

			var hostBuilder = WebHost
				.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((context, config) =>
				{
					var env = context.HostingEnvironment;

					// config.SetBasePath(Directory.GetCurrentDirectory());

					config
						.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
						.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

					if (env.IsDevelopment())
					{
						// todo: remove
						config.AddUserSecrets(Assembly.Load(new AssemblyName(env.ApplicationName)), optional: true);
					}

					config
						.AddEnvironmentVariables()
						.AddCommandLine(args);

					// build temp config and preload connection strings
					config.Build().SetLinq2DbDefaultSettings();

					config.AddDbConfiguration();
				})
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
				var logger = scope.ServiceProvider.GetService<ILogger<Program>>();

				var modules = host.Services.GetServices<IModule>().ToArray();

				foreach (var module in modules)
				{
					if (module is IStartupTask startupTask)
					{
						logger.LogInformation("Running {module} startup task", module);

						await startupTask.Run(CancellationToken.None);
					}
				}

				// todo: run startup tasks from modules or sort IStartupTask's by module initialization order
				var tasks = scope.ServiceProvider.GetServices<IStartupTask>().ToArray();

				foreach (var task in tasks)
				{
					logger.LogInformation("Running {task} startup task", task);

					await task.Run(CancellationToken.None);
				}
			}

			await host.RunAsync();
		}

		// todo: run migration in production before application
		public static async Task Migrate(string[] args)
		{
			var hostBuilder = Microsoft.Extensions.Hosting.Host
				.CreateDefaultBuilder()
				.ConfigureAppConfiguration((context, config) =>
				{
					var env = context.HostingEnvironment;

					config
						.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
						.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
						.AddUserSecrets(Assembly.Load(new AssemblyName(env.ApplicationName)), optional: true)
						.AddEnvironmentVariables()
						.AddCommandLine(args);

					config.Build().SetLinq2DbDefaultSettings();
				})
				.ConfigureServices((context, services) =>
				{
					services.BindOptions<MigrationOptions>(context.Configuration);

					services.AddSingleton<IMigrationRunner, DbMigrationRunner>();
					services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
					services.AddSingleton<EmbeddedResourceProvider, EmbeddedResourceProvider>();
				})
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
				var migrator = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

				await migrator.Run(CancellationToken.None);
			}

			host.Dispose();

			NamedServiceCollectionExtensions.ClearRegistrations();
		}
	}
}
