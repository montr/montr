using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Impl.Services;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;

namespace Montr.Automate.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IAutomationProviderRegistry, DefaultAutomationProviderRegistry>();
			services.AddSingleton<IAutomationService, DefaultAutomationService>();

			services.AddTransient<IRepository<Automation>, DbAutomationRepository>();

			services.AddNamedTransient<IAutomationConditionProvider, FieldAutomationConditionProvider>(FieldAutomationCondition.TypeCode);
			services.AddNamedTransient<IAutomationActionProvider, SetFieldAutomationActionProvider>(SetFieldAutomationAction.TypeCode);
			services.AddNamedTransient<IAutomationActionProvider, NotifyByEmailAutomationActionProvider>(NotifyByEmailAutomationAction.TypeCode);
		}
	}
}
