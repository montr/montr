using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Metadata.Models;

namespace Montr.Metadata.Impl.Services
{
	public class FieldMetadataJsonOptionsConfigurator : IConfigureOptions<MvcNewtonsoftJsonOptions>
	{
		private readonly JsonTypeProvider<FieldMetadata> _typeProvider;

		public FieldMetadataJsonOptionsConfigurator(JsonTypeProvider<FieldMetadata> typeProvider)
		{
			_typeProvider = typeProvider;
		}

		public void Configure(MvcNewtonsoftJsonOptions options)
		{
			options.SerializerSettings.Converters.Add(new PolymorphicNewtonsoftJsonConverter<FieldMetadata>(x => x.Type, _typeProvider.Map));
		}
	}
}
