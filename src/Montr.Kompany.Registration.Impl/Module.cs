using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Kompany.Registration.Impl.Services;

namespace Montr.Kompany.Registration.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IWebApplicationBuilderConfigurator
	{
		public void Configure(WebApplicationBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<CompanyRequestValidationHelper, CompanyRequestValidationHelper>();
		}
	}
}
