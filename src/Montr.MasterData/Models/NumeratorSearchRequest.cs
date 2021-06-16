using System;

namespace Montr.MasterData.Models
{
	public class NumeratorSearchRequest : ClassifierSearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid? EntityTypeUid { get; set; }

		public bool? IsAutoNumbering { get; set; }
	}
}
