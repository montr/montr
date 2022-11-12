using System.Reflection;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Settings.Services.Designers
{
	public class TextFieldDesigner : AbstractSettingsDesigner<TextField>
	{
		protected override async Task<TextField> GetMetadataInternal(PropertyInfo property)
		{
			var result = await base.GetMetadataInternal(property);

			// todo: add support for RequiredAttribute.AllowEmptyStrings

			return result;
		}
	}
}
