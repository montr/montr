using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Montr.Core.Services.Implementations;

namespace Montr.Settings.Services.Implementations
{
	public class SettingsJsonOptionsConfigurator : IConfigureOptions<MvcNewtonsoftJsonOptions>
	{
		private readonly JsonTypeProvider<ISettingsType> _typeProvider;

		public SettingsJsonOptionsConfigurator(JsonTypeProvider<ISettingsType> typeProvider)
		{
			_typeProvider = typeProvider;
		}

		public void Configure(MvcNewtonsoftJsonOptions options)
		{
			var converter = new PolymorphicNewtonsoftJsonConverter<ISettingsType>(x => x.TypeCode, _typeProvider)
			{
				UsePopulate = true
			};

			options.SerializerSettings.Converters.Add(converter);
		}
	}
}
