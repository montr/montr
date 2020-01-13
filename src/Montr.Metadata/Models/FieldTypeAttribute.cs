using System;

namespace Montr.Metadata.Models
{
	[AttributeUsage(AttributeTargets.Class)]
	public class FieldTypeAttribute : Attribute
	{
		public FieldTypeAttribute(string typeCode, Type providerType)
		{
			TypeCode = typeCode;
			ProviderType = providerType;
		}

		public string TypeCode { get; }

		public Type ProviderType { get; }

		public bool IsSystem { get; set; }
	}
}
