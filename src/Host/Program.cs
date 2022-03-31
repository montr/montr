using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Host.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Impl.Services;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Host
{
	public static class Program
	{
		public static async Task Main(string[] args)
		{
			await Migrate(args);

			var options = new WebApplicationOptions
			{
				ApplicationName = typeof(Program).Assembly.FullName,
				ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
				WebRootPath = "wwwroot",
				Args = args
			};

			var appBuilder = WebApplication.CreateBuilder(options);

			appBuilder.Host
				.UseLogging()
				.ConfigureAppConfiguration((context, config) =>
				{
					var env = context.HostingEnvironment;

					config
						.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
						.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
						.AddUserSecrets(Assembly.Load(new AssemblyName(env.ApplicationName)),
							optional: true) // todo: remove
						.AddEnvironmentVariables()
						.AddCommandLine(args);
				});

			appBuilder.WebHost
				.UseDbSettings(reloadOnChange: true)
				.UseSentry()
				.UseStartup<Startup>();

			var app = appBuilder.Build();

			await app.RunAsync();
		}

		// todo: run migration in production in separate process before application
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
				.UseLogging();

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
