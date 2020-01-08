using System;

namespace Montr.Metadata.Services
{
	public interface IFieldProvider
	{
		Type FieldType { get; }

		bool Validate(object value, out object parsed, out string[] errors);

		/// <summary>
		/// Read string value from storage and return object.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		object Read(string value);

		/// <summary>
		/// Write object value to string for storage.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		string Write(object value);
	}
}
