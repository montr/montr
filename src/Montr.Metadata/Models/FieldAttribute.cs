using System;

namespace Montr.Metadata.Models
{
	[AttributeUsage(AttributeTargets.Property)]
	public class FieldAttribute : Attribute
	{
		public FieldAttribute(string typeCode)
		{
			TypeCode = typeCode;
		}

		public string TypeCode { get; }
	}
}
