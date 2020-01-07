using System;

namespace Montr.Metadata.Services
{
	public interface IFieldProvider
	{
		Type FieldType { get; }
	}
}
