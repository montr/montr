using System;

namespace Montr.MasterData.Models
{
	public class NumeratorEntity
	{
		public Guid NumeratorUid { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public string EntityName { get; set; }
	}
}
