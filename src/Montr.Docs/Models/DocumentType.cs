using System.Diagnostics;
using Montr.MasterData.Models;

namespace Montr.Docs.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class DocumentType : Classifier
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		public static readonly string EntityTypeCode = nameof(DocumentType);
	}
}
