using System;

namespace Montr.Core.Services
{
	public interface IJsonSerializer
	{
		string Serialize(object value);

		object Deserialize(string value, Type type);

		T Deserialize<T>(string value);
	}
}
