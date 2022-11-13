using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.Implementations
{
	public class ClassifierJsonOptionsConfigurator : IConfigureOptions<MvcNewtonsoftJsonOptions>
	{
		private readonly JsonTypeProvider<Classifier> _typeProvider;

		public ClassifierJsonOptionsConfigurator(JsonTypeProvider<Classifier> typeProvider)
		{
			_typeProvider = typeProvider;
		}

		public void Configure(MvcNewtonsoftJsonOptions options)
		{
			var converter = new PolymorphicNewtonsoftJsonConverter<Classifier>(x => x.Type, _typeProvider)
			{
				ConvertMode = PolymorphicJsonConvertMode.BaseAndInheritors,
				UseBaseTypeIfTypeNotFound = true,
				UsePopulate = true
			};

			options.SerializerSettings.Converters.Add(converter);
		}
	}
}
