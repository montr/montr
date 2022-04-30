using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<IStartupTask, ImportDefaultLocaleStringListStartupTask>();

			appBuilder.Services.AddSingleton<IConfigurationManager, DefaultConfigurationManager>();
			appBuilder.Services.AddSingleton<IContentService, DefaultContentService>();
			appBuilder.Services.AddSingleton<IContentProvider, DefaultContentProvider>();
			appBuilder.Services.AddTransient<IPermissionProvider, PermissionProvider>();

			appBuilder.Services.AddSingleton<ICurrentUserProvider, DefaultCurrentUserProvider>();
			appBuilder.Services.AddSingleton<EmbeddedResourceProvider, EmbeddedResourceProvider>();
			appBuilder.Services.AddSingleton<LocaleStringSerializer, LocaleStringSerializer>();

			appBuilder.Services.AddSingleton<ILocaleStringImporter, DbLocaleStringImporter>();
			appBuilder.Services.AddSingleton<IRepository<LocaleString>, DbLocaleStringRepository>();
			appBuilder.Services.AddSingleton<IAuditLogService, DbAuditLogService>();
			appBuilder.Services.AddSingleton<ISettingsRepository, DbSettingsRepository>();

			appBuilder.Services.AddSingleton<IRepository<EntityStatus>, DbEntityStatusRepository>();
			appBuilder.Services.AddSingleton<IEntityStatusProvider, DefaultEntityStatusProvider>();

			appBuilder.Services.AddSingleton<IPermissionResolver, DefaultPermissionResolver>();
			appBuilder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

			appBuilder.Services.AddTransient<IConfigurationService, DefaultConfigurationService>();
			appBuilder.Services.AddTransient<IRecipeExecutor, DefaultRecipeExecutor>();
			appBuilder.Services.AddTransient<IEntityRelationService, DbEntityRelationService>();
		}
	}
}
