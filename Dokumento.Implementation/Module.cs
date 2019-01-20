using Dokumento.Implementation.Services;
using Dokumento.Services;
using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;

namespace Dokumento.Implementation
{
	public class Module : IModule
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IDocumentRepository, DbDocumentRepository>();
		}
	}
}
