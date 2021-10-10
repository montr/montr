using System;
using System.Diagnostics;

namespace Montr.Tasks.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class TaskModel
	{
		private string DebuggerDisplay => $"{StatusCode}, {Name}";

		public Guid? Uid { get; set; }

		public Guid? CompanyUid { get; set; }

		public Guid TaskTypeUid { get; set; }

		public string StatusCode { get; set; }

		public string Number { get; set; }

		public string Name { get; set; }

		public string Url { get; set; }
	}
}
