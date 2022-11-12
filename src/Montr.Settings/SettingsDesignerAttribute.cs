using System;

namespace Montr.Settings
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SettingsDesignerAttribute : Attribute
	{
		public SettingsDesignerAttribute(Type designerType)
		{
			DesignerType = designerType;
		}

		public Type DesignerType { get; }
	}
}
