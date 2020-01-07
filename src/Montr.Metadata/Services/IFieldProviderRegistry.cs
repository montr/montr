using System.Collections.Generic;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IFieldProviderRegistry
	{
		void AddFieldType(string fieldType, IFieldProvider fieldProvider);

		IFieldProvider GetFieldTypeProvider(string fieldType);

		IList<FieldType> GetFieldTypes();
	}
}
