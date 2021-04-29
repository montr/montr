using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
					Code = TypeCode,
					Name = "Roles",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = ClassifierMetadata.GetDefaultFields().ToList()
			};
		}
	}
}
