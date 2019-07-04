using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class ClassifierGroup
	{
		// todo: move to Constants
		public const string DefaultRootCode = "default";

		private string DebuggerDisplay => $"{Code}, {Name}";

		public Guid Uid { get; set; }

		public string Code { get; set; }

		public Guid? ParentUid { get; set; }

		public string ParentCode { get; set; }

		public string Name { get; set; }

		public IList<ClassifierGroup> Children { get; set; }
	}
}
