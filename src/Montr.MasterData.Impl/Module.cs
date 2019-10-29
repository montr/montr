using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IRepository<ClassifierType>, DbClassifierTypeRepository>();
			services.AddTransient<IRepository<ClassifierTree>, DbClassifierTreeRepository>();
			services.AddTransient<IRepository<Classifier>, DbClassifierRepository>();

			services.AddTransient<IClassifierTypeService, DefaultClassifierTypeService>();
			services.AddTransient<IClassifierTreeService, DefaultClassifierTreeService>();
		}
	}
}
