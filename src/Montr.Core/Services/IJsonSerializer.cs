using System.Text.Json;

namespace Montr.Core.Services
{
	public interface IJsonSerializer
	{
		string Serialize(object value);

		T Deserialize<T>(string value);
	}

	// todo: add tests
	public class DefaultJsonSerializer : IJsonSerializer
	{
		public string Serialize(object value)
		{
			return JsonSerializer.Serialize(value);
		}

		public T Deserialize<T>(string value)
		{
			throw new System.NotImplementedException();
		}
	}
}
