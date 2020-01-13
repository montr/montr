using System;
using System.Collections.Generic;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IFieldProviderRegistry
	{
		void AddFieldType(Type fieldType);

		IFieldProvider GetFieldTypeProvider(string fieldType);

		IList<FieldType> GetFieldTypes();
	}
}
