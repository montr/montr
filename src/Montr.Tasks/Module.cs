using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Tasks.Models;
using Montr.Tasks.Services;
using Montr.Tasks.Services.Implementations;

namespace Montr.Tasks
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.BindOptions<TasksOptions>(appBuilder.Configuration);

			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();

			appBuilder.Services.AddSingleton<IContentProvider, ContentProvider>();

			appBuilder.Services.AddTransient<IPermissionProvider, TasksPermissionProvider>();

			appBuilder.Services.AddTransient<ITaskService, DbTaskService>();
			appBuilder.Services.AddTransient<IRepository<TaskModel>, DbTaskRepository>();

			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbTaskTypeRepository>(ClassifierTypeCode.TaskType);

			appBuilder.Services.AddNamedTransient<IAutomationActionProvider, CreateTaskAutomationActionProvider>(CreateTaskAutomationAction.TypeCode);
		}
	}
}
