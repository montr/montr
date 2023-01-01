using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Montr.Metadata.Models;
using Montr.Metadata.Services.Implementations;

namespace Montr.Metadata.Services.Designers
{
	public abstract class AbstractFieldDesigner<TField> : IFieldDesigner where TField : FieldMetadata, new()
	{
		public async Task<FieldMetadata> GetMetadata(PropertyInfo property, CancellationToken cancellationToken)
		{
			return await GetMetadataInternal(property, cancellationToken);
		}

		protected virtual Task<TField> GetMetadataInternal(PropertyInfo property, CancellationToken cancellationToken)
		{
			var key = property.Name[..1].ToLowerInvariant() + property.Name[1..];

			var required = property.GetCustomAttribute<RequiredAttribute>() != null;

			var result = new TField
			{
				Key = key,
				Name = FieldNameUtils.BuildSettingsName(property.Name),
				Required = required
			};

			// todo: add support for [DefaultValue]

			return Task.FromResult(result) ;
		}
	}
}
