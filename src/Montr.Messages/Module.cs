using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Messages.Services;

namespace Montr.Messages
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.BindOptions<Options>(appBuilder.Configuration);

			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
		}
	}
}
