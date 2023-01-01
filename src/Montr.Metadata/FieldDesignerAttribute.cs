using System;

namespace Montr.Metadata
{
	[AttributeUsage(AttributeTargets.Property)]
	public class FieldDesignerAttribute : Attribute
	{
		public FieldDesignerAttribute(Type designerType)
		{
			DesignerType = designerType;
		}

		public Type DesignerType { get; }
	}
}
