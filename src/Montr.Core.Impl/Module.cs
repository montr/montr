using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddTransient<IStartupTask, ImportDefaultLocaleStringListStartupTask>();

			services.AddSingleton<IConfigurationManager, DefaultConfigurationManager>();
			services.AddSingleton<IContentService, DefaultContentService>();
			services.AddSingleton<IContentProvider, DefaultContentProvider>();
			services.AddTransient<IPermissionProvider, PermissionProvider>();

			services.AddSingleton<ICurrentUserProvider, DefaultCurrentUserProvider>();
			services.AddSingleton<EmbeddedResourceProvider, EmbeddedResourceProvider>();
			services.AddSingleton<LocaleStringSerializer, LocaleStringSerializer>();

			services.AddSingleton<ILocaleStringImporter, DbLocaleStringImporter>();
			services.AddSingleton<IRepository<LocaleString>, DbLocaleStringRepository>();
			services.AddSingleton<IAuditLogService, DbAuditLogService>();
			services.AddSingleton<ISettingsRepository, DbSettingsRepository>();

			services.AddSingleton<IRepository<EntityStatus>, DbEntityStatusRepository>();
			services.AddSingleton<IEntityStatusProvider, DefaultEntityStatusProvider>();

			services.AddSingleton<IPermissionResolver, DefaultPermissionResolver>();
			services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

			services.AddTransient<IConfigurationService, DefaultConfigurationService>();
			services.AddTransient<IRecipeExecutor, DefaultRecipeExecutor>();
		}
	}
}
