using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Models
{
	[FieldType(Code, typeof(ClassifierFieldProvider))]
	public class ClassifierField : FieldMetadata<ClassifierField.Properties>
	{
		public const string Code = "classifier";

		public override string Type => Code;

		public class Properties
		{
			public string TypeCode { get; set; }
		}
	}

	[FieldType(Code, typeof(ClassifierGroupFieldProvider), IsSystem = true)]
	public class ClassifierGroupField : FieldMetadata<ClassifierGroupField.Properties>
	{
		public const string Code = "classifier-group";

		public override string Type => Code;

		public class Properties
		{
			public string TypeCode { get; set; }

			public string TreeCode { get; set; }
		}
	}

	[FieldType(TypeCode, typeof(DefaultFieldProvider<SelectClassifierTypeField, string>), IsSystem = true)]
	public class SelectClassifierTypeField : FieldMetadata
	{
		public const string TypeCode = "select-classifier-type";

		public override string Type => TypeCode;
	}
}
