using System.Text;
using System.Threading;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
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
	[Module( Dependencies = new [] { typeof(Idx.Module) })]
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddHangfire(config =>
			{
				config.UsePostgreSqlStorage(
					configuration.GetConnectionString(Data.Constants.DefaultConnectionStringName),
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
				Authorization = new [] { new DashboardAuthorizationFilter() },
				// IsReadOnlyFunc = (context) => true,
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

		// todo: replace with real auth
		public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
		{
			public bool Authorize(DashboardContext context)
			{
				var httpContext = context.GetHttpContext();

				// Allow all authenticated users to see the Dashboard (potentially dangerous).
				return httpContext.User.Identity.IsAuthenticated;
			}
		}
	}
}
