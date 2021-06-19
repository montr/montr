using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Kompany.Impl.Services;
using Montr.Kompany.Services;
using Montr.MasterData.Services;

namespace Montr.Kompany.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddNamedTransient<IClassifierRepository, DbCompanyRepository>(ClassifierTypeCode.Company);

			services.AddSingleton<ICurrentCompanyProvider, DefaultCurrentCompanyProvider>();
		}
	}
}
