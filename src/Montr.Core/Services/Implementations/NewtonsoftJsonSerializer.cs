using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Montr.Core.Services.Implementations
{
	public class NewtonsoftJsonSerializer : IJsonSerializer
	{
		private readonly JsonSerializerSettings _settings = new();

		public static void SetupDefaultSettings(JsonSerializerSettings settings)
		{
			settings.MaxDepth = 64;

			// settings.DefaultValueHandling = DefaultValueHandling.Ignore; // do not use - zeros in numbers ignored also
			settings.Converters.Add(new StringEnumConverter());
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
		}

		public NewtonsoftJsonSerializer()
		{
			SetupDefaultSettings(_settings);
		}

		public string Serialize(object value)
		{
			return JsonConvert.SerializeObject(value, _settings);
		}

		public object Deserialize(string value, Type type)
		{
			return JsonConvert.DeserializeObject(value, type, _settings);
		}

		public T Deserialize<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value, _settings);
		}
	}
}
