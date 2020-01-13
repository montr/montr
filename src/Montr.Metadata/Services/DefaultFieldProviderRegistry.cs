using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public class DefaultFieldProviderRegistry : IFieldProviderRegistry
	{
		private readonly ConcurrentDictionary<string, Type> _providerMap = new ConcurrentDictionary<string, Type>();
		private readonly ConcurrentDictionary<string, FieldType> _typeMap = new ConcurrentDictionary<string, FieldType>();

		public void AddFieldType(Type fieldType)
		{
			var attribute = fieldType.GetCustomAttribute<FieldTypeAttribute>();

			// todo: synchronize
			_providerMap[attribute.TypeCode] = attribute.ProviderType;

			_typeMap[attribute.TypeCode] = new FieldType
			{
				Code = attribute.TypeCode,
				IsSystem = attribute.IsSystem
			};
		}

		public IFieldProvider GetFieldTypeProvider(string fieldType)
		{
			var providerType = _providerMap[fieldType];

			return (IFieldProvider)Activator.CreateInstance(providerType);
		}

		public IList<FieldType> GetFieldTypes()
		{
			// todo: localize
			// todo: clone
			return _typeMap.Values.ToList();
		}
	}
}
