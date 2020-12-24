using System;

namespace Montr.MasterData.Models
{
	public class NumeratorEntity
	{
		public bool IsAutoNumbering { get; set; }

		public Guid? NumeratorUid { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public string EntityName { get; set; }
	}
}
