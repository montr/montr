using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Montr.Metadata.Models;
using Montr.Settings.Services.Designers;

namespace Montr.Settings.Services.Implementations
{
	public class DefaultSettingsMetadataProvider : ISettingsMetadataProvider
	{
		private readonly IServiceProvider _serviceProvider;

		private static readonly Dictionary<Type, Type> DefaultDesignerTypes = new Dictionary<Type, Type>
		{
			{ typeof(int), typeof(NumberFieldDesigner) },
			{ typeof(bool), typeof(BooleanFieldDesigner) },
		};

		private static readonly Type DefaultDesignerType = typeof(TextFieldDesigner);

		public DefaultSettingsMetadataProvider(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task<ICollection<FieldMetadata>> GetMetadata(Type type)
		{
			var result = new List<FieldMetadata>();

			var properties = type.GetProperties();

			foreach (var property in properties)
			{
				var designerType = GetDesignerType(property);

				var designer = (ISettingsDesigner)ActivatorUtilities.CreateInstance(_serviceProvider, designerType);

				var metadata = await designer.GetMetadata(property);

				result.Add(metadata);
			}

			return await Task.FromResult(result);
		}

		private static Type GetDesignerType(PropertyInfo property)
		{
			var designerAttribute = property.GetCustomAttribute<SettingsDesignerAttribute>();

			if (designerAttribute != null)
			{
				return designerAttribute.DesignerType;
			}

			if (DefaultDesignerTypes.TryGetValue(property.PropertyType, out var result))
			{
				return result;
			}

			return DefaultDesignerType;
		}
	}
}
