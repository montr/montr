using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services.Designers
{
	public class NumberFieldDesigner : AbstractFieldDesigner<NumberField>
	{
		protected override async Task<NumberField> GetMetadataInternal(PropertyInfo property, CancellationToken cancellationToken)
		{
			var result = await base.GetMetadataInternal(property, cancellationToken);

			var rangeAttribute = property.GetCustomAttribute<RangeAttribute>();

			if (rangeAttribute != null)
			{
				result.Props.Min = Convert.ToDecimal(rangeAttribute.Minimum);
				result.Props.Max = Convert.ToDecimal(rangeAttribute.Maximum);
			}

			return result;
		}
	}
}
