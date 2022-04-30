using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Kompany.Registration.Impl.Services;

namespace Montr.Kompany.Registration.Impl
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<CompanyRequestValidationHelper, CompanyRequestValidationHelper>();
		}
	}
}
