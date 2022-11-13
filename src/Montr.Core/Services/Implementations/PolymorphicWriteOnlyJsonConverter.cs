using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Montr.Core.Services.Implementations
{
	public class PolymorphicWriteOnlyJsonConverter<T> : JsonConverter<T>
	{
		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			throw new NotImplementedException();

			// return (T)JsonSerializer.Deserialize(ref reader, typeof(Montr.Core.Models.TextField), options);
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			JsonSerializer.Serialize(writer, value, value.GetType(), options);
		}
	}
}
