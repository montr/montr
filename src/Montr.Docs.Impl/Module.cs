using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Docs.Impl.Services;
using Montr.Docs.Services;

namespace Montr.Docs.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IDocumentRepository, DbDocumentRepository>();
		}
	}
}
