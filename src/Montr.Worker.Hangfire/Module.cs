using System.Text;
using System.Threading;
using Hangfire;
using Hangfire.Common;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
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

			app.UseHangfireDashboard(options: new DashboardOptions
			{
				AppPath = "/dashboard",
				DisplayNameFunc = (context, job) => FormatJobName(job)
			});
		}

		private static string FormatJobName(Job job)
		{
			var result = new StringBuilder();

			foreach (var arg in job.Args)
			{
				if (arg is CancellationToken) continue;

				if (result.Length > 0) result.Append(", ");

				result.Append(arg);
			}

			return result.ToString();
		}
	}
}
