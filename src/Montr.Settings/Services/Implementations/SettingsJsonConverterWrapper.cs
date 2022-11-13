using System;
using System.Linq;
using Montr.Core.Services.Implementations;
using Newtonsoft.Json;

namespace Montr.Settings.Services.Implementations
{
	/// <summary>
	/// Marker-wrapper to deserialize settings objects not implemented ISettingsType.
	/// Real deserialization happened in PolymorphicNewtonsoftJsonConverter<ISettingsType>.
	/// </summary>
	public class SettingsJsonConverterWrapper : JsonConverter
	{
		private static readonly Type WrappedType = typeof(PolymorphicNewtonsoftJsonConverter<ISettingsType>);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var wrapped = serializer.Converters.FirstOrDefault(x => x.GetType() == WrappedType);

			if (wrapped != null) return wrapped.ReadJson(reader, objectType, existingValue, serializer);

			throw new InvalidOperationException($"{WrappedType} should be registered to use SettingsJsonConverterWrapper.");
		}

		public override bool CanConvert(Type objectType)
		{
			return true;
		}
	}
}
