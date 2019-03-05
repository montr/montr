using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Services;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.Modularity;

namespace Montr.MasterData.Impl
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IRepository<ClassifierType>, DbClassifierTypeRepository>();
			services.AddTransient<IRepository<Classifier>, DbClassifierRepository>();
		}
	}
}
