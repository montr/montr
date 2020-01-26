using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Montr.Core.Services
{
	public class MigrationHostedService : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;

		public MigrationHostedService(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _serviceProvider.GetService<IMigrationRunner>().Run(cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	}
}
