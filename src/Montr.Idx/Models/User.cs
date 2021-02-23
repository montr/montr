using System;
using System.Diagnostics;

namespace Montr.Idx.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class User
	{
		private string DebuggerDisplay => $"{UserName}";

		public Guid? Uid { get; set; }

		public string UserName { get; set; }

		public string LastName { get; set; }

		public string FirstName { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public string SecurityStamp { get; set; }

		public string Url { get; set; }
	}
}
