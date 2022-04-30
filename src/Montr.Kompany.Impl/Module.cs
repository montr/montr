using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Kompany.Impl.Services;
using Montr.Kompany.Services;
using Montr.MasterData.Services;

namespace Montr.Kompany.Impl
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbCompanyRepository>(ClassifierTypeCode.Company);

			appBuilder.Services.AddSingleton<ICurrentCompanyProvider, DefaultCurrentCompanyProvider>();
		}
	}
}
