using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class NumeratorEntitySearchRequest : SearchRequest
	{
		public string EntityTypeCode { get; set; }

		public Guid? EntityUid { get; set; }

		public Guid? NumeratorUid { get; set; }
	}
}
