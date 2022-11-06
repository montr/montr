using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Kompany.Services;
using Montr.Kompany.Services.Implementations;
using Montr.MasterData.Services;
using RegisterClassifierTypeStartupTask = Montr.Kompany.Services.Implementations.RegisterClassifierTypeStartupTask;

namespace Montr.Kompany
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddSingleton<ICurrentCompanyProvider, DefaultCurrentCompanyProvider>();
			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbCompanyRepository>(ClassifierTypeCode.Company);
			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
		}
	}
}
