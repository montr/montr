using System.Diagnostics;
using System.Linq;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;

namespace Montr.Idx.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Role : Classifier
	{
		private string DebuggerDisplay => $"{Name}";

		public Role()
		{
			Type = ClassifierTypeCode.Role;
		}

		public string ConcurrencyStamp { get; set; }

		public static RegisterClassifierType GetDefaultMetadata()
		{
			return new()
			{
				Item = new ClassifierType
				{
					Code = ClassifierTypeCode.Role,
					Name = "Roles",
					HierarchyType = HierarchyType.Groups,
					IsSystem = true
				},
				Fields = ClassifierMetadata.GetDefaultFields().ToList()
			};
		}
	}
}
