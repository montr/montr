﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Automate.Impl.Models;
using Montr.Automate.Impl.Services;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;
using Montr.Metadata.Services;

namespace Montr.Automate.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IStartupTask, AutomationJsonOptionsInitializer>();

			services.AddSingleton<IAutomationProviderRegistry, DefaultAutomationProviderRegistry>();
			services.AddSingleton<IAutomationService, DefaultAutomationService>();
			services.AddSingleton<IAutomationRunner, DefaultAutomationRunner>();
			services.AddSingleton<IRecipientResolver, DefaultRecipientResolver>();

			services.AddTransient<IRepository<Automation>, DbAutomationRepository>();

			services.AddNamedTransient<IAutomationConditionProvider, GroupAutomationConditionProvider>(GroupAutomationCondition.TypeCode);
			services.AddNamedTransient<IAutomationConditionProvider, FieldAutomationConditionProvider>(FieldAutomationCondition.TypeCode);
			services.AddNamedTransient<IAutomationActionProvider, SetFieldAutomationActionProvider>(SetFieldAutomationAction.TypeCode);
			services.AddNamedTransient<IAutomationActionProvider, NotifyByEmailAutomationActionProvider>(NotifyByEmailAutomationAction.TypeCode);

			services.AddSingleton<JsonTypeProvider<AutomationCondition>>();
			services.AddSingleton<JsonTypeProvider<AutomationAction>>();
			services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, AutomationJsonOptionsConfigurator>();

		}

		public void Configure(IApplicationBuilder app)
		{
			app.ConfigureMetadata(options =>
			{
				options.Registry.AddFieldType(typeof(AutomationConditionListField));
			});
		}
	}
}
