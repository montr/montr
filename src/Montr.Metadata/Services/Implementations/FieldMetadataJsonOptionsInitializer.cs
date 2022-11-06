using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services.Implementations
{
	public class FieldMetadataJsonOptionsInitializer : IStartupTask
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly JsonTypeProvider<FieldMetadata> _typeProvider;

		public FieldMetadataJsonOptionsInitializer(IServiceProvider serviceProvider, JsonTypeProvider<FieldMetadata> typeProvider)
		{
			_serviceProvider = serviceProvider;
			_typeProvider = typeProvider;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				var fieldProviderRegistry = scope.ServiceProvider.GetRequiredService<IFieldProviderRegistry>();

				foreach (var fieldType in fieldProviderRegistry.GetFieldTypes())
				{
					_typeProvider.Map[fieldType.Code] = fieldProviderRegistry.GetFieldTypeProvider(fieldType.Code).FieldType;
				}
			}

			return Task.CompletedTask;
		}
	}
}
