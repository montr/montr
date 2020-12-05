using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Montr.Metadata.Models;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Classifier : IFieldDataContainer
	{
		public Classifier()
		{
			IsActive = true;
		}

		private string DebuggerDisplay => $"[{Type}] {Code}, {Name}";

		public static readonly string TypeCode = nameof(Classifier);

		public string Type { get; set; }

		public Guid? Uid { get; set; }

		public string StatusCode { get; set; }

		[Required]
		public string Code { get; set; }

		[Required]
		public string Name { get; set; }

		public bool IsActive { get; set; }

		public bool IsSystem { get; set; }

		public Guid? ParentUid { get; set; }

		public string ParentCode { get; set; }

		public string Url { get; set; }

		public FieldData Fields { get; set; }
	}
}
