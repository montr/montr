using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public static class WebHostExtensions
	{
		public static async Task Run(this IWebHost host, CancellationToken cancellationToken = default)
		{
			await RunStartupTasks(host, cancellationToken);

			await host.RunAsync(cancellationToken);
		}

		private static async Task RunStartupTasks(IWebHost host, CancellationToken cancellationToken = default)
		{
			using (var scope = host.Services.CreateScope())
			{
				var logger = scope.ServiceProvider.GetService<ILogger<ModuleRunner>>();

				var modules = scope.ServiceProvider.GetServices<IModule>().ToArray();

				foreach (var module in modules)
				{
					if (module is IStartupTask startupTask)
					{
						logger.LogInformation("Running {module} startup task", module);

						await startupTask.Run(cancellationToken);
					}
				}

				// todo: run startup tasks from modules or sort IStartupTask's by module initialization order
				// fixme: startup tasks already ordered, because they are registered in ordered modules
				foreach (var task in scope.ServiceProvider.GetServices<IStartupTask>())
				{
					logger.LogInformation("Running {task} startup task", task);

					await task.Run(cancellationToken);
				}
			}
		}
	}
}
