using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Services;
using Montr.Modularity;

namespace Montr.Core
{
	public class Module : IModule
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IDateTimeProvider, DefaultDateTimeProvider>();
			services.AddSingleton<IJsonSerializer, DefaultJsonSerializer>();
			services.AddSingleton<IUnitOfWorkFactory, TransactionScopeUnitOfWorkFactory>();
		}
	}
}
