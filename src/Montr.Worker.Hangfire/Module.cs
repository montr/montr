using System.Text;
using System.Threading;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Worker.Hangfire.Services;
using Montr.Worker.Services;
using Newtonsoft.Json;

namespace Montr.Worker.Hangfire
{
	[Module( DependsOn = new [] { typeof(Idx.Module) })]
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IContentProvider, ContentProvider>();

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
				AppPath = ClientRoutes.Dashboard,
				Authorization = new [] { new DashboardAuthorizationFilter() },
				// IsReadOnlyFunc = (context) => true,
				DisplayNameFunc = FormatJobName
			});
		}

		private static string FormatJobName(DashboardContext context, Job job)
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

		public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
		{
			public bool Authorize(DashboardContext context)
			{
				var httpContext = context.GetHttpContext();

				var authorizationService = httpContext.RequestServices.GetService<IAuthorizationService>();

				var authResult = authorizationService.AuthorizePermission(httpContext.User,
					Permission.GetCode(typeof(Permissions.ViewDashboard))).ConfigureAwait(false).GetAwaiter().GetResult();

				return authResult?.Succeeded == true;
			}
		}
	}
}
