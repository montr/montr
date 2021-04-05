using System.Diagnostics;
using Montr.MasterData.Models;

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
	}
}
