using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services.Designers
{
	public class TextFieldDesigner : AbstractFieldDesigner<TextField>
	{
		protected override async Task<TextField> GetMetadataInternal(PropertyInfo property, CancellationToken cancellationToken)
		{
			var result = await base.GetMetadataInternal(property, cancellationToken);

			// todo: add support for RequiredAttribute.AllowEmptyStrings

			return result;
		}
	}
}
