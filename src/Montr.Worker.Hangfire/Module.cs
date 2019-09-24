using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Web;
using Montr.Worker.Hangfire.Services;
using Montr.Worker.Services;
using Newtonsoft.Json;

namespace Montr.Worker.Hangfire
{
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddHangfire(config =>
			{
				config.UsePostgreSqlStorage(
					configuration.GetConnectionString("Default"),
					new PostgreSqlStorageOptions { PrepareSchemaIfNecessary = false });

				config.UseSerializerSettings(
					new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
			});

			services.AddSingleton<IBackgroundJobManager, HangfireBackgroundJobManager>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseHangfireServer();
			app.UseHangfireDashboard();
		}
	}
}
