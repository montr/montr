using Montr.MasterData.Services.Implementations;
using Montr.Metadata.Models;
using Montr.Metadata.Services;
using Montr.Metadata.Services.Implementations;

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

	[FieldType(TypeCode, typeof(DefaultFieldProvider<ClassifierTypeField, string>), IsSystem = true)]
	public class ClassifierTypeField : FieldMetadata
	{
		public const string TypeCode = "select-classifier-type";

		public override string Type => TypeCode;
	}
}
