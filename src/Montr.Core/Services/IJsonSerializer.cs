using Newtonsoft.Json;

namespace Montr.Core.Services
{
	public interface IJsonSerializer
	{
		string Serialize(object value, string configName = null);

		T Deserialize<T>(string value);
	}

	public class DefaultJsonSerializer : IJsonSerializer
	{
		public string Serialize(object value, string configName = null)
		{
			return JsonConvert.SerializeObject(value);
		}

		public T Deserialize<T>(string value)
		{
			throw new System.NotImplementedException();
		}
	}
}
