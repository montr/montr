using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierGroupSearchRequest : SearchRequest
	{
		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }

		public string TypeCode { get; set; }

		public string TreeCode { get; set; }
	}
}
