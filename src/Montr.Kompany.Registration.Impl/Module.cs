using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Kompany.Registration.Impl.Services;

namespace Montr.Kompany.Registration.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<CompanyRequestValidationHelper, CompanyRequestValidationHelper>();
		}
	}
}
