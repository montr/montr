using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierLinkSearchRequest : SearchRequest
	{
		public string TypeCode { get; set; }

		public Guid? GroupUid { get; set; }

		public Guid? ItemUid { get; set; }
	}
}
