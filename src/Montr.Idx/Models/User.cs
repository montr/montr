using System.Diagnostics;
using Montr.MasterData.Models;

namespace Montr.Idx.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class User : Classifier
	{
		public new static readonly string TypeCode = nameof(User).ToLower();

		private string DebuggerDisplay => $"{UserName}";

		public string UserName { get; set; }

		public string LastName { get; set; }

		public string FirstName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string ConcurrencyStamp { get; set; }
	}
}
