using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Designers;

namespace Montr.Metadata.Services.Implementations
{
	public class DefaultDataAnnotationMetadataProvider : IDataAnnotationMetadataProvider
	{
		private readonly IServiceProvider _serviceProvider;

		private static readonly Dictionary<Type, Type> DefaultDesignerTypes = new()
		{
			{ typeof(int), typeof(NumberFieldDesigner) },
			{ typeof(bool), typeof(BooleanFieldDesigner) },
		};

		private static readonly Type DefaultDesignerType = typeof(TextFieldDesigner);

		public DefaultDataAnnotationMetadataProvider(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task<ICollection<FieldMetadata>> GetMetadata(Type type, CancellationToken cancellationToken)
		{
			var result = new List<FieldMetadata>();

			var properties = type.GetProperties();

			foreach (var property in properties)
			{
				var designerType = GetDesignerType(property);

				var designer = (IFieldDesigner)ActivatorUtilities.CreateInstance(_serviceProvider, designerType);

				var metadata = await designer.GetMetadata(property, cancellationToken);

				result.Add(metadata);
			}

			return await Task.FromResult(result);
		}

		private static Type GetDesignerType(PropertyInfo property)
		{
			// first, get designer from type specified in [FieldDesigner] on property
			var designerAttribute = property.GetCustomAttribute<FieldDesignerAttribute>();

			if (designerAttribute != null)
			{
				return designerAttribute.DesignerType;
			}

			// then, get default designer by property type
			if (DefaultDesignerTypes.TryGetValue(property.PropertyType, out var result))
			{
				return result;
			}

			// otherwise get default designer
			return DefaultDesignerType;
		}
	}
}
