using System;

namespace Montr.MasterData.Models
{
	public class GenerateNumberRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid? EntityTypeUid { get; set; }

		public Guid? EntityUid { get; set; }
	}
}
