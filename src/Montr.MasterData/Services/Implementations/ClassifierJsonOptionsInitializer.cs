using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.Implementations
{
	public class ClassifierJsonOptionsInitializer : IStartupTask
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly JsonTypeProvider<Classifier> _typeProvider;

		public ClassifierJsonOptionsInitializer(IServiceProvider serviceProvider, JsonTypeProvider<Classifier> typeProvider)
		{
			_serviceProvider = serviceProvider;
			_typeProvider = typeProvider;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var classifierRepositoryFactory = scope.ServiceProvider.GetRequiredService<INamedServiceFactory<IClassifierRepository>>();

				foreach (var name in classifierRepositoryFactory.GetNames())
				{
					_typeProvider.Map[name] = classifierRepositoryFactory.GetRequiredService(name).ClassifierType;
				}
			}

			return Task.CompletedTask;
		}
	}
}
