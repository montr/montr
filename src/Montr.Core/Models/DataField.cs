using System;
using System.Collections.Concurrent;

namespace Montr.Core.Models
{
	public class DataFieldTypes
	{
		public static readonly ConcurrentDictionary<string, Type> Map = new ConcurrentDictionary<string, Type>();

		static DataFieldTypes()
		{
			Map[Boolean] = typeof(BooleanField);
			Map[Text] = typeof(TextField);
			Map[TextArea] = typeof(TextAreaField);
			Map[Password] = typeof(PasswordField);
			Map[Number] = typeof(NumberField);
			Map[Decimal] = typeof(DecimalField);
			Map[Date] = typeof(DateField);
			Map[Time] = typeof(TimeField);
			Map[DateTime] = typeof(DateTimeField);
			Map[Select] = typeof(SelectField);
			Map[Classifier] = typeof(ClassifierField);
			Map[ClassifierGroup] = typeof(ClassifierGroupField);
			Map[File] = typeof(FileField);
		}

		public static readonly string Boolean = "boolean";
		public static readonly string Text = "text";
		public static readonly string TextArea = "textarea";
		public static readonly string Password = "password";
		public static readonly string Number = "number";
		public static readonly string Decimal = "decimal";
		public static readonly string Date = "date";
		public static readonly string Time = "time";
		public static readonly string DateTime = "datetime";
		public static readonly string Select = "select";
		public static readonly string Classifier = "classifier";
		public static readonly string ClassifierGroup = "classifier-group";
		public static readonly string File = "file";
	}

	public class FieldType
	{
		public string Code { get; set; }

		public string Name { get; set; }

		public string Icon { get; set; }
	}

	public abstract class DataField
	{
		public abstract string Type { get; }

		public Guid Uid { get; set; }

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

	public abstract class DataField<TProps> : DataField where TProps : new()
	{
		protected DataField()
		{
			Properties= new TProps();
		}

		public TProps Properties { get; set; }

		public override Type GetPropertiesType()
		{
			return typeof(TProps);
		}

		public override object GetProperties()
		{
			return Properties;
		}

		public override void SetProperties(object value)
		{
			Properties = (TProps)value;
		}
	}

	public class BooleanField : DataField
	{
		public override string Type => DataFieldTypes.Boolean;
	}

	public class TextField : DataField
	{
		public override string Type => DataFieldTypes.Text;
	}

	// todo join with TextField?
	public class TextAreaField : DataField<TextAreaField.Props>
	{
		public override string Type => "textarea";

		public class Props
		{
			public byte? Rows { get; set; }
		}
	}

	public class PasswordField : DataField
	{
		public override string Type => "password";
	}

	public class NumberField : DataField<NumberField.Props>
	{
		public override string Type => "number";

		public class Props
		{
			public decimal? Min { get; set; }

			public decimal? Max { get; set; }
		}
	}

	// todo: merge with number?
	public class DecimalField : DataField<DecimalField.Props>
	{
		public override string Type => "decimal";

		public class Props
		{
			public decimal? Min { get; set; }

			public decimal? Max { get; set; }

			public byte? Precision { get; set; }
		}
	}

	public class DateField : DataField
	{
		public override string Type => "date";
	}

	// todo join with Date?
	public class TimeField : DataField
	{
		public override string Type => "time";
	}

	// todo join with Date?
	public class DateTimeField : DataField
	{
		public override string Type => "datetime";
	}

	public class SelectField : DataField<SelectField.Props>
	{
		public override string Type => "select";

		public class Props
		{
			public SelectFieldOption[] Options { get; set; }
		}
	}

	public class SelectFieldOption
	{
		public string Value { get; set; }

		public string Name { get; set; }
	}

	public class ClassifierGroupField : DataField<ClassifierGroupField.Props>
	{
		public override string Type => "classifier-group";

		public class Props
		{
			public string TypeCode { get; set; }

			public string TreeCode { get; set; }
		}
	}

	public class ClassifierField : DataField<ClassifierField.Props>
	{
		public override string Type => "classifier";

		public class Props
		{
			public string TypeCode { get; set; }
		}
	}

	public class FileField : DataField
	{
		public override string Type => "file";
	}
}
