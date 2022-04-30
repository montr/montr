using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Automate.Impl.Models;
using Montr.Automate.Impl.Services;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Metadata.Services;

namespace Montr.Automate.Impl
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule, IAppConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddSingleton<IStartupTask, AutomationJsonOptionsInitializer>();

			appBuilder.Services.AddTransient<IAutomationProviderRegistry, DefaultAutomationProviderRegistry>();
			appBuilder.Services.AddTransient<IAutomationContextProvider, DefaultAutomationContextProvider>();
			appBuilder.Services.AddTransient<IAutomationRunner, DefaultAutomationRunner>();
			appBuilder.Services.AddTransient<IRecipientResolver, DefaultRecipientResolver>();

			appBuilder.Services
				.AddNamedTransient<IClassifierRepository, DbAutomationRepository>(ClassifierTypeCode.Automation);

			appBuilder.Services
				.AddNamedTransient<IAutomationConditionProvider, GroupAutomationConditionProvider>(GroupAutomationCondition.TypeCode)
				.AddNamedTransient<IAutomationConditionProvider, FieldAutomationConditionProvider>(FieldAutomationCondition.TypeCode)
				.AddNamedTransient<IAutomationActionProvider, SetFieldAutomationActionProvider>(SetFieldAutomationAction.TypeCode)
				.AddNamedTransient<IAutomationActionProvider, NotifyByEmailAutomationActionProvider>(NotifyByEmailAutomationAction.TypeCode);

			appBuilder.Services.AddSingleton<JsonTypeProvider<AutomationCondition>>();
			appBuilder.Services.AddSingleton<JsonTypeProvider<AutomationAction>>();
			appBuilder.Services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, AutomationJsonOptionsConfigurator>();
		}

		public void Configure(IApp app)
		{
			app.ConfigureMetadata(options =>
			{
				options.Registry.AddFieldType(typeof(AutomationActionListField));
				options.Registry.AddFieldType(typeof(AutomationConditionListField));
			});
		}
	}
}
