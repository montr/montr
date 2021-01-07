using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Kompany.Impl.Services;
using Montr.Kompany.Models;
using Montr.Kompany.Services;

namespace Montr.Kompany.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IStartupTask, RegisterDocumentTypeStartupTask>();

			services.AddTransient<IRepository<Company>, DbCompanyRepository>();

			services.AddSingleton<ICurrentCompanyProvider, DefaultCurrentCompanyProvider>();
		}
	}
}
