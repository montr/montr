using Newtonsoft.Json;

namespace Montr.Core.Services
{
	public class NewtonsoftJsonSerializer : IJsonSerializer
	{
		public string Serialize(object value)
		{
			return JsonConvert.SerializeObject(value);
		}

		public T Deserialize<T>(string value)
		{
			throw new System.NotImplementedException();
		}
	}
}
