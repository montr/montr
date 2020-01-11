using System;

namespace Montr.Metadata.Models
{
	public abstract class FieldMetadata
	{
		public abstract string Type { get; }

		public Guid? Uid { get; set; }

		public string Key { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

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

	public class BooleanField : FieldMetadata
	{
		public static readonly string TypeCode = "boolean";

		public override string Type => TypeCode;
	}

	public class TextField : FieldMetadata
	{
		public static readonly string TypeCode = "text";

		public override string Type => TypeCode;
	}

	// todo join with TextField?
	public class TextAreaField : FieldMetadata<TextAreaField.Properties>
	{
		public static readonly string TypeCode = "textarea";

		public override string Type => TypeCode;

		public class Properties
		{
			public byte? Rows { get; set; }
		}
	}

	public class PasswordField : FieldMetadata
	{
		public static readonly string TypeCode = "password";

		public override string Type => TypeCode;
	}

	public class NumberField : FieldMetadata<NumberField.Properties>
	{
		public static readonly string TypeCode = "number";

		public override string Type => TypeCode;

		public class Properties
		{
			public decimal? Min { get; set; }

			public decimal? Max { get; set; }
		}
	}

	// todo: merge with number?
	public class DecimalField : FieldMetadata<DecimalField.Properties>
	{
		public static readonly string TypeCode = "decimal";

		public override string Type => TypeCode;

		public class Properties
		{
			public decimal? Min { get; set; }

			public decimal? Max { get; set; }

			public byte? Precision { get; set; }
		}
	}

	public class DateField : FieldMetadata<DateField.Properties>
	{
		public static readonly string TypeCode = "date";

		public override string Type => TypeCode;

		public class Properties
		{
			public bool IncludeTime { get; set; }
		}
	}

	public class TimeField : FieldMetadata<TimeField.Properties>
	{
		public static readonly string TypeCode = "time";

		public override string Type => TypeCode;

		public class Properties
		{
		}
	}

	// todo: add options for select single/multiple, dropdown/radio/checkboxes
	public class SelectField : FieldMetadata<SelectField.Properties>
	{
		public static readonly string TypeCode = "select";

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

	public class DesignSelectOptionsField : FieldMetadata
	{
		public static readonly string TypeCode = "select-options";

		public override string Type => TypeCode;
	}

	public class ClassifierField : FieldMetadata<ClassifierField.Properties>
	{
		public static readonly string Code = "classifier";

		public override string Type => Code;

		public class Properties
		{
			public string TypeCode { get; set; }
		}
	}

	public class ClassifierGroupField : FieldMetadata<ClassifierGroupField.Properties>
	{
		public static readonly string Code = "classifier-group";

		public override string Type => Code;

		public class Properties
		{
			public string TypeCode { get; set; }

			public string TreeCode { get; set; }
		}
	}

	public class FileField : FieldMetadata
	{
		public static readonly string TypeCode = "file";

		public override string Type => TypeCode;
	}
}
