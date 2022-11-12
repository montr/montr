using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Settings.Services.Designers
{
	public class NumberFieldDesigner : AbstractSettingsDesigner<NumberField>
	{
		protected override async Task<NumberField> GetMetadataInternal(PropertyInfo property)
		{
			var result = await base.GetMetadataInternal(property);

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
