using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Implementation.Services;
using Montr.Core.Services;
using Montr.Modularity;

namespace Montr.Core.Implementation
{
	public class ModuleInitializer : IModuleInitializer
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IAuditLogService, DbAuditLogService>();
		}
	}
}
