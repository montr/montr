using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class NumeratorSearchRequest : SearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid? EntityTypeUid { get; set; }

		public Guid? Uid { get; set; }
	}
}
