using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Kompany.Services;

namespace Montr.Kompany
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IWebApplicationBuilderConfigurator
	{
		public void Configure(WebApplicationBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
		}
	}
}
