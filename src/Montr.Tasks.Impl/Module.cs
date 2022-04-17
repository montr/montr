using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Tasks.Impl.Services;
using Montr.Tasks.Models;
using Montr.Tasks.Services;

namespace Montr.Tasks.Impl
{
	public class Module : IModule, IAppBuilderConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddSingleton<IContentProvider, ContentProvider>();

			appBuilder.Services.AddTransient<IPermissionProvider, PermissionProvider>();

			appBuilder.Services.AddTransient<ITaskService, DbTaskService>();
			appBuilder.Services.AddTransient<IRepository<TaskModel>, DbTaskRepository>();

			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbTaskTypeRepository>(ClassifierTypeCode.TaskType);

			appBuilder.Services.AddNamedTransient<IAutomationActionProvider, CreateTaskAutomationActionProvider>(CreateTaskAutomationAction.TypeCode);
		}
	}
}
