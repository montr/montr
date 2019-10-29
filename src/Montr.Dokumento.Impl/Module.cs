using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Dokumento.Impl.Services;
using Montr.Dokumento.Services;

namespace Montr.Dokumento.Impl
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IDocumentRepository, DbDocumentRepository>();
		}
	}
}
