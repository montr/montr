using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Docs.Impl.Services;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.MasterData.Services;

namespace Montr.Docs.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IRepository<DocumentType>, DbDocumentTypeRepository>();
			services.AddSingleton<IRepository<Document>, DbDocumentRepository>();
			services.AddSingleton<IDocumentService, DbDocumentService>();

			services.AddTransient<INumberTagResolver, DocumentNumberTagResolver>();
		}
	}
}
