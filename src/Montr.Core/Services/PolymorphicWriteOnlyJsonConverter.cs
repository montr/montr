using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public class PolymorphicWriteOnlyJsonConverter<T> : JsonConverter<T> // where T : new()
	{
		public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			// throw new NotImplementedException();

			return (T)JsonSerializer.Deserialize(ref reader, typeof(StringField), options);
		}

		public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
		{
			JsonSerializer.Serialize(writer, value, value.GetType(), options);
		}
	}
}
