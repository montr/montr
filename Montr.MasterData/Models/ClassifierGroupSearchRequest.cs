using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierGroupSearchRequest : SearchRequest
	{
		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		// public string TreeCode { get; set; }

		public Guid? ParentUid { get; set; }

		public Guid? FocusUid { get; set; }

		public bool ExpandSingleChild { get; set; }
	}
}
