using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Messages.Services;

namespace Montr.Messages
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IAppBuilderConfigurator, IStartupTask
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.BindOptions<Options>(appBuilder.Configuration);

			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
		}

		public Task Run(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
