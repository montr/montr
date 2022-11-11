using System;

namespace Montr.Settings
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SettingsDesignerAttribute : Attribute
	{
		public SettingsDesignerAttribute(string typeCode)
		{
			TypeCode = typeCode;
		}

		public string TypeCode { get; }
	}
}
