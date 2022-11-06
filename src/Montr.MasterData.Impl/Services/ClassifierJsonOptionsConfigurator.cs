using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.Services
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
			options.SerializerSettings.Converters.Add(new PolymorphicNewtonsoftJsonConverterWithPopulate<Classifier>(x => x.Type, _typeProvider.Map));
		}
	}
}
