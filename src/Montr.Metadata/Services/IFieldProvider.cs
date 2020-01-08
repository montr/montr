using System;

namespace Montr.Metadata.Services
{
	public interface IFieldProvider
	{
		Type FieldType { get; }

		object Read(string value);

		string Write(object value);
	}
}
