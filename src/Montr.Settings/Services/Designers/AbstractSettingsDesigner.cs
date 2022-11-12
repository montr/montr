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
			var result = new TField
			{
				Key = property.Name,
				Name = property.Name,
				Required = property.GetCustomAttribute<RequiredAttribute>() != null
			};

			// todo: add support for [DefaultValue]

			return Task.FromResult(result) ;
		}
	}
}
