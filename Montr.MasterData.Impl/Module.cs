﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Modularity;

namespace Montr.MasterData.Impl
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			// no-op, to load assembly in domain
		}
	}
}
