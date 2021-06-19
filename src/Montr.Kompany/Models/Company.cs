using System.Collections.Generic;
using System.Diagnostics;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Kompany.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Company : Classifier
	{
		public Company()
		{
			Type = ClassifierTypeCode.Company;
		}

		private string DebuggerDisplay => $"{ConfigCode}, {Name}";

		public static readonly string EntityTypeCode = nameof(Company);

		public string ConfigCode { get; set; }

		public static RegisterClassifierType GetDefaultMetadata()
		{
			return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = ClassifierTypeCode.Company,
					Name = "Companies",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Code", DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Name", Required = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 2 } },
				}
			};
		}
	}
}
