using System;
using Newtonsoft.Json;

namespace Montr.Core.Services
{
	public class NewtonsoftJsonSerializer : IJsonSerializer
	{
		public string Serialize(object value)
		{
			return JsonConvert.SerializeObject(value);
		}

		public object Deserialize(string value, Type type)
		{
			return JsonConvert.DeserializeObject(value, type);
		}

		public T Deserialize<T>(string value)
		{
			return JsonConvert.DeserializeObject<T>(value);
		}
	}
}
