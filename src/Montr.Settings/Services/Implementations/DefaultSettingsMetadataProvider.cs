using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Settings.Services.Implementations
{
	public class DefaultSettingsMetadataProvider : ISettingsMetadataProvider
	{
		public async Task<ICollection<FieldMetadata>> GetMetadata(Type type)
		{
			var result = new List<FieldMetadata>();

			var properties = type.GetProperties();

			foreach (var property in properties)
			{
				FieldMetadata field;

				if (property.PropertyType == typeof(bool))
				{
					field = new BooleanField();
				}
				else
				{
					field = new TextField();
				}

				field.Key = property.Name;
				field.Name = property.Name;
				field.Required = property.GetCustomAttribute<RequiredAttribute>() != null;

				result.Add(field);
			}

			return await Task.FromResult(result);
		}
	}
}
