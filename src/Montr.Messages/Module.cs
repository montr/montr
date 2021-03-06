﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Messages.Services;

namespace Montr.Messages
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IStartupTask
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.BindOptions<Options>(configuration);

			services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
		}

		public Task Run(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}
