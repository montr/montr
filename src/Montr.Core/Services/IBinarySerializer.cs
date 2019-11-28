using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Montr.Core.Services
{
	public interface IBinarySerializer
	{
		byte[] Serialize(object value);

		T Deserialize<T>(byte[] value);
	}

	public class DefaultBinarySerializer : IBinarySerializer
	{
		public byte[] Serialize(object value)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			if (value is string)
			{
				return Encoding.UTF8.GetBytes((string)value);
			}

			using (var stream = new MemoryStream())
			{
				new BinaryFormatter().Serialize(stream, value);

				return stream.ToArray();
			}
		}

		public T Deserialize<T>(byte[] value)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			if (typeof(T) == typeof(string))
			{
				return (T)(object)Encoding.UTF8.GetString(value);
			}

			using (var stream = new MemoryStream(value))
			{
				return (T)new BinaryFormatter().Deserialize(stream);
			}
		}
	}
}
