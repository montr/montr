using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IRepository<FieldMetadata>, DbFieldMetadataRepository>();
			services.AddSingleton<IFieldDataRepository, DbFieldDataRepository>();
			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
		}
	}
}
