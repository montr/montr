﻿using System.Threading;
using System.Threading.Tasks;
using Host.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core;
using Montr.Core.Impl.Services;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Host
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await Migrate(args);

			var options = new WebApplicationOptions
			{
				// ApplicationName = typeof(Program).Assembly.FullName,
				// ContentRootPath = Path.GetFullPath(Directory.GetCurrentDirectory()),
				WebRootPath = "wwwroot",
				Args = args
			};

			var appBuilder = WebApplication.CreateBuilder(options);

			appBuilder.Host
				.UseLogging()
				.UseDefaultConfiguration(args);

			appBuilder.WebHost
				.UseDbSettings(reloadOnChange: true)
				.UseSentry();

			// todo: create in modules builder
			var logger = LoggerFactory.Create(builder => { }).CreateLogger<Program>();
			var modules = appBuilder.Services.AddModules(logger);

			var appBuilderWrapper = new WebApplicationBuilderWrapper(appBuilder, modules);

			foreach (var module in modules)
			{
				(module as IAppBuilderConfigurator)?.Configure(appBuilderWrapper);
			}

			var app = appBuilder.Build();

			var appWrapper = new WebApplicationWrapper(app);

			foreach (var module in modules)
			{
				(module as IAppConfigurator)?.Configure(appWrapper);
			}

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapRazorPages();
				endpoints.MapControllers();
				endpoints.MapFallbackToController("Index", "Home");
				// endpoints.MapFallbackToFile("Home/Index.cshtml");
				// endpoints.MapHub<MyChatHub>()
				// endpoints.MapGrpcService<MyCalculatorService>()
				endpoints.MapDefaultControllerRoute();
			});

			await app.RunAsync();
		}

		// todo: run migration in production in separate process before application
		public static async Task Migrate(string[] args)
		{
			var hostBuilder = Microsoft.Extensions.Hosting.Host
				.CreateDefaultBuilder()
				.UseLogging()
				.UseDefaultConfiguration(args)
				.ConfigureServices((context, services) =>
				{
					services.BindOptions<MigrationOptions>(context.Configuration);

					services.AddSingleton<IMigrationRunner, DbMigrationRunner>();
					services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
					services.AddSingleton<EmbeddedResourceProvider, EmbeddedResourceProvider>();
				});

			using (var host = hostBuilder.Build())
			{
				using (var scope = host.Services.CreateScope())
				{
					var services = scope.ServiceProvider;

					services.GetRequiredService<IConfiguration>().SetLinq2DbDefaultSettings();

					var migrator = services.GetRequiredService<IMigrationRunner>();

					await migrator.Run(CancellationToken.None);
				}
			}

			NamedServiceCollectionExtensions.ClearRegistrations();
		}
	}
}
