﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Implementation.Services;
using Montr.Core.Services;
using Montr.Modularity;

namespace Montr.Core.Implementation
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IAuditLogService, DbAuditLogService>();
		}
	}
}
