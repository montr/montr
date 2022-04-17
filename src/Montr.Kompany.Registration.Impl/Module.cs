using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Kompany.Registration.Impl.Services;

namespace Montr.Kompany.Registration.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IAppBuilderConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<CompanyRequestValidationHelper, CompanyRequestValidationHelper>();
		}
	}
}
