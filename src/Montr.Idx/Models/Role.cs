using System.Collections.Generic;
using System.Diagnostics;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.Idx.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Role : Classifier
	{
		public new static readonly string TypeCode = nameof(Role).ToLower();

		private string DebuggerDisplay => $"{Name}";

		public Role()
		{
			Type = TypeCode;
		}

		public string ConcurrencyStamp { get; set; }

		public static RegisterClassifierType GetDefaultMetadata()
		{
			return new()
			{
				Item = new ClassifierType
				{
					Code = Role.TypeCode,
					Name = "Роли",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = new List<FieldMetadata>
				{
					new TextField
					{
						Key = "code", Name = "Код", Required = true, Active = true, DisplayOrder = 10, System = true
					},
					new TextAreaField
					{
						Key = "name", Name = "Наименование", Required = true, Active = true, DisplayOrder = 20,
						System = true, Props = new TextAreaField.Properties { Rows = 10 }
					},
				}
			};
		}
	}
}
