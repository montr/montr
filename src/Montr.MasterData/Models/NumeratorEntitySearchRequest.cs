using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class NumeratorEntitySearchRequest : SearchRequest
	{
		public Guid NumeratorUid { get; set; }
	}
}
