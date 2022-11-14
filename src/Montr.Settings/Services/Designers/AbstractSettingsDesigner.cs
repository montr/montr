using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Settings.Services.Designers
{
	public abstract class AbstractSettingsDesigner<TField> : ISettingsDesigner where TField : FieldMetadata, new()
	{
		public async Task<FieldMetadata> GetMetadata(PropertyInfo property)
		{
			return await GetMetadataInternal(property);
		}

		protected virtual Task<TField> GetMetadataInternal(PropertyInfo property)
		{
			var key = property.Name[..1].ToLowerInvariant() + property.Name[1..];

			var required = property.GetCustomAttribute<RequiredAttribute>() != null;

			var result = new TField
			{
				Key = key,
				Name = property.Name,
				Required = required
			};

			// todo: add support for [DefaultValue]

			return Task.FromResult(result) ;
		}
	}
}
