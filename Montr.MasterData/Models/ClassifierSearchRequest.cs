using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierSearchRequest : SearchRequest
	{
		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		// public string TreeCode { get; set; }

		public Guid? GroupUid { get; set; }

		public string Depth { get; set; }

		public Guid? Uid { get; set; }
	}
}
