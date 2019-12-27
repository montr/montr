using System;
using System.Collections.Concurrent;

namespace Montr.Core.Models
{
	public class DataFieldType
	{
		public static readonly ConcurrentDictionary<string, Type> Map = new ConcurrentDictionary<string, Type>();

		static DataFieldType()
		{
			Map[Boolean] = typeof(BooleanField);
			Map[String] = typeof(StringField);
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
		public static readonly string String = "string";
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
	}

	public class BooleanField : DataField
	{
		public override string Type => DataFieldType.Boolean;
	}

	public class StringField : DataField
	{
		public override string Type => DataFieldType.String;

		public bool Autosize { get; set; }
	}

	// todo join with StringField?
	public class TextAreaField : StringField
	{
		public override string Type => "textarea";

		public byte? Rows { get; set; }
	}

	public class PasswordField : DataField
	{
		public override string Type => "password";
	}

	public class NumberField : DataField
	{
		public override string Type => "number";

		public long? Min { get; set; }

		public long? Max { get; set; }
	}

	public class DecimalField : DataField
	{
		public override string Type => "decimal";

		public decimal? Min { get; set; }

		public decimal? Max { get; set; }

		public byte? Precision { get; set; }
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

	public class SelectField : DataField
	{
		public override string Type => "select";

		public SelectFieldOption[] Options { get; set; }
	}

	public class SelectFieldOption
	{
		public string Value { get; set; }

		public string Name { get; set; }
	}

	public class ClassifierGroupField : DataField
	{
		public override string Type => "classifier-group";

		public string TypeCode { get; set; }

		public string TreeCode { get; set; }
	}

	public class ClassifierField : DataField
	{
		public override string Type => "classifier";

		public string TypeCode { get; set; }
	}

	public class FileField : DataField
	{
		public override string Type => "file";
	}
}
