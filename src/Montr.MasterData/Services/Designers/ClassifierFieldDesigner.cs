using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;
using Montr.Metadata.Services.Designers;

namespace Montr.MasterData.Services.Designers
{
	public class ClassifierFieldDesigner : AbstractFieldDesigner<ClassifierField>
	{
		protected override async Task<ClassifierField> GetMetadataInternal(PropertyInfo property, CancellationToken cancellationToken)
		{
			var result = await base.GetMetadataInternal(property, cancellationToken);

			var classifierFieldAttribute = property.GetCustomAttribute<ClassifierFieldAttribute>();

			if (classifierFieldAttribute != null)
			{
				result.Props.TypeCode = classifierFieldAttribute.ClassifierTypeCode;
			}

			return result;
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class ClassifierFieldAttribute : Attribute
	{
		public string ClassifierTypeCode { get; }

		public ClassifierFieldAttribute(string classifierTypeCode)
		{
			ClassifierTypeCode = classifierTypeCode;
		}
	}
}
