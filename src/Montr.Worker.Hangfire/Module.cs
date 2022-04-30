using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.AspNetCore;
using Hangfire.Common;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Worker.Hangfire.Services;
using Montr.Worker.Services;

namespace Montr.Worker.Hangfire
{
	[Module( DependsOn = new [] { typeof(Idx.Module) })]
	// ReSharper disable once UnusedType.Global
	public class Module : IModule, IAppConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddSingleton<IContentProvider, ContentProvider>();

			appBuilder.Services.AddTransient<JobActivator, AspNetCoreJobActivator>();

			appBuilder.Services
				.AddHangfireServer((_, _) => { })
				.AddHangfire((_, config) =>
				{
					config.UseSerilogLogProvider();
					config.UseDefaults(appBuilder.Configuration);
				});

			appBuilder.Services.AddTransient<IBackgroundJobManager, HangfireBackgroundJobManager>();
		}

		public void Configure(IApp app)
		{
			app.UseHangfireDashboard(options: new DashboardOptions
			{
				AppPath = ClientRoutes.Dashboard,
				AsyncAuthorization = new [] { new DashboardAuthorizationFilter() },
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

		public class DashboardAuthorizationFilter : IDashboardAsyncAuthorizationFilter
		{
			public async Task<bool> AuthorizeAsync(DashboardContext context)
			{
				var httpContext = context.GetHttpContext();

				var authorizationService = httpContext.RequestServices.GetService<IAuthorizationService>();

				var authResult = await authorizationService
					.AuthorizePermission(httpContext.User, Permission.GetCode(typeof(Permissions.ViewDashboard)));

				return authResult?.Succeeded == true;
			}
		}
	}
}
