using System;
using System.Text.Json;

namespace Montr.Core.Services.Impl
{
	// todo: add tests
	public class DefaultJsonSerializer : IJsonSerializer
	{
		public string Serialize(object value)
		{
			return JsonSerializer.Serialize(value);
		}

		public object Deserialize(string value, Type type)
		{
			throw new NotImplementedException();
		}

		public T Deserialize<T>(string value)
		{
			throw new System.NotImplementedException();
		}
	}
}
