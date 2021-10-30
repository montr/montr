using System;
using Montr.Metadata.Services;

namespace Montr.Metadata.Models
{
	public abstract class FieldMetadata
	{
		protected FieldMetadata()
		{
			Active = true;
		}

		public abstract string Type { get; }

		public Guid? Uid { get; set; }

		public string Key { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Tooltip { get; set; }

		public string Placeholder { get; set; }

		public string Icon { get; set; }

		public bool Multiple { get; set; }

		public bool Active { get; set; }

		public bool System { get; set; }

		public bool Readonly { get; set; }

		public bool Required { get; set; }

		public int DisplayOrder { get; set; }

		public virtual Type GetPropertiesType()
		{
			return null;
		}

		public virtual object GetProperties()
		{
			return null;
		}

		public virtual void SetProperties(object value)
		{
		}
	}

	public abstract class FieldMetadata<TProps> : FieldMetadata where TProps : new()
	{
		protected FieldMetadata()
		{
			Props= new TProps();
		}

		public TProps Props { get; set; }

		public override Type GetPropertiesType()
		{
			return typeof(TProps);
		}

		public override object GetProperties()
		{
			return Props;
		}

		public override void SetProperties(object value)
		{
			Props = (TProps)value;
		}
	}

	/// <summary>
	/// todo: move to question type
	/// </summary>
	[FieldType(TypeCode, typeof(SectionFieldProvider))]
	public class SectionField : FieldMetadata
	{
		public const string TypeCode = "section";

		public override string Type => TypeCode;
	}

	[FieldType(TypeCode, typeof(DefaultFieldProvider<BooleanField, bool>))]
	public class BooleanField : FieldMetadata
	{
		public const string TypeCode = "boolean";

		public override string Type => TypeCode;
	}

	[FieldType(TypeCode, typeof(DefaultFieldProvider<TextField, string>))]
	public class TextField : FieldMetadata
	{
		public const string TypeCode = "text";

		public override string Type => TypeCode;
	}

	// todo join with TextField?
	[FieldType(TypeCode, typeof(TextAreaFieldProvider))]
	public class TextAreaField : FieldMetadata<TextAreaField.Properties>
	{
		public const string TypeCode = "textarea";

		public override string Type => TypeCode;

		public class Properties
		{
			public byte? Rows { get; set; }
		}
	}

	[FieldType(TypeCode, typeof(DefaultFieldProvider<PasswordField, string>))]
	public class PasswordField : FieldMetadata
	{
		public const string TypeCode = "password";

		public override string Type => TypeCode;
	}

	[FieldType(TypeCode, typeof(NumberFieldProvider))]
	public class NumberField : FieldMetadata<NumberField.Properties>
	{
		public const string TypeCode = "number";

		public override string Type => TypeCode;

		public class Properties
		{
			public decimal? Min { get; set; }

			public decimal? Max { get; set; }
		}
	}

	// todo: merge with number?
	[FieldType(TypeCode, typeof(DecimalFieldProvider))]
	public class DecimalField : FieldMetadata<DecimalField.Properties>
	{
		public const string TypeCode = "decimal";

		public override string Type => TypeCode;

		public class Properties
		{
			public decimal? Min { get; set; }

			public decimal? Max { get; set; }

			public byte? Precision { get; set; }
		}
	}

	[FieldType(TypeCode, typeof(DateFieldProvider))]
	public class DateField : FieldMetadata<DateField.Properties>
	{
		public const string TypeCode = "date";

		public override string Type => TypeCode;

		public class Properties
		{
			public bool IncludeTime { get; set; }
		}
	}

	[FieldType(TypeCode, typeof(TimeFieldProvider))]
	public class TimeField : FieldMetadata<TimeField.Properties>
	{
		public const string TypeCode = "time";

		public override string Type => TypeCode;

		public class Properties
		{
		}
	}

	// todo: add options for select single/multiple, dropdown/radio/checkboxes
	[FieldType(TypeCode, typeof(SelectFieldProvider))]
	public class SelectField : FieldMetadata<SelectField.Properties>
	{
		public const string TypeCode = "select";

		public override string Type => TypeCode;

		public class Properties
		{
			public SelectFieldOption[] Options { get; set; }
		}
	}

	public class SelectFieldOption
	{
		public string Value { get; set; }

		public string Name { get; set; }
	}

	[FieldType(TypeCode, typeof(DefaultFieldProvider<DesignSelectOptionsField, SelectFieldOption>), IsSystem = true)]
	public class DesignSelectOptionsField : FieldMetadata
	{
		public const string TypeCode = "select-options";

		public override string Type => TypeCode;
	}

	[FieldType(TypeCode, typeof(DefaultFieldProvider<FileField, string>))]
	public class FileField : FieldMetadata
	{
		public const string TypeCode = "file";

		public override string Type => TypeCode;
	}
}
