using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Settings.Services.Implementations
{
	public class DefaultSettingsMetadataProvider : ISettingsMetadataProvider
	{
		private readonly IFieldProviderRegistry _fieldProviderRegistry;

		private static readonly Dictionary<Type, string> DefaultDesignerTypeCodes = new Dictionary<Type, string>
		{
			{ typeof(int), NumberField.TypeCode },
			{ typeof(bool), BooleanField.TypeCode },
		};

		private static readonly string DefaultDesignerTypeCode = TextField.TypeCode;

		public DefaultSettingsMetadataProvider(IFieldProviderRegistry fieldProviderRegistry)
		{
			_fieldProviderRegistry = fieldProviderRegistry;
		}

		public async Task<ICollection<FieldMetadata>> GetMetadata(Type type)
		{
			var result = new List<FieldMetadata>();

			var properties = type.GetProperties();

			foreach (var property in properties)
			{
				var typeCode = GetDesignerTypeCode(property);

				var fieldTypeProvider = _fieldProviderRegistry.GetFieldTypeProvider(typeCode);

				var field = (FieldMetadata)Activator.CreateInstance(fieldTypeProvider.FieldType);

				if (field != null)
				{
					field.Key = property.Name;
					field.Name = property.Name;
					field.Required = property.GetCustomAttribute<RequiredAttribute>() != null;

					result.Add(field);
				}
			}

			return await Task.FromResult(result);
		}

		private static string GetDesignerTypeCode(PropertyInfo property)
		{
			var designerAttribute = property.GetCustomAttribute<SettingsDesignerAttribute>();

			if (designerAttribute != null)
			{
				return designerAttribute.TypeCode;
			}

			if (DefaultDesignerTypeCodes.TryGetValue(property.PropertyType, out var result))
			{
				return result;
			}

			return DefaultDesignerTypeCode;
		}
	}
}
