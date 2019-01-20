﻿using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;

namespace Montr.Data.Linq2Db
{
	public class ModuleInitializer : IModuleInitializer
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<IDbContextFactory, DefaultDbContextFactory>();
		}
	}
}
