using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Montr.Metadata.Models;

namespace Montr.MasterData.Models
{
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
	public class Classifier : IFieldDataContainer
	{
		private string DebuggerDisplay => $"{Code}, {Name}";

		public static readonly string EntityTypeCode = typeof(Classifier).Name;

		public Guid? Uid { get; set; }

		public string StatusCode { get; set; }

		[Required]
		public string Code { get; set; }

		[Required]
		public string Name { get; set; }

		public Guid? ParentUid { get; set; }

		public string ParentCode { get; set; }

		public string Url { get; set; }

		public FieldData Fields { get; set; }
	}
}
