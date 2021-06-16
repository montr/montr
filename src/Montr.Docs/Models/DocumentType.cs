using System.Collections.Generic;
using System.Diagnostics;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Docs.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class DocumentType : Classifier
	{
		public DocumentType()
		{
			Type = ClassifierTypeCode.DocumentType;
		}

		private string DebuggerDisplay => $"{Code}, {Name}";

		// todo: remove (?)
		public static readonly string EntityTypeCode = nameof(DocumentType);

		public static RegisterClassifierType GetDefaultMetadata()
		{
			return new RegisterClassifierType
			{
				Item = new ClassifierType
				{
					Code = ClassifierTypeCode.DocumentType,
					Name = "Document types",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField { Key = "code", Name = "Code", Required = true, DisplayOrder = 10, System = true },
					new TextAreaField { Key = "name", Name = "Name", Required = true, DisplayOrder = 20, System = true, Props = new TextAreaField.Properties { Rows = 2 } },
				}
			};
		}
	}
}
