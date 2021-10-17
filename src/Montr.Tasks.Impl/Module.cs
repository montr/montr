using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Tasks.Impl.Services;
using Montr.Tasks.Models;
using Montr.Tasks.Services;

namespace Montr.Tasks.Impl
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IContentProvider, ContentProvider>();

			services.AddTransient<IPermissionProvider, PermissionProvider>();

			services.AddTransient<ITaskService, DbTaskService>();
			services.AddTransient<IRepository<TaskModel>, DbTaskRepository>();
		}
	}
}
