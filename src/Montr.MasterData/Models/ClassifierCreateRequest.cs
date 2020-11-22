using System;

namespace Montr.MasterData.Models
{
	public class ClassifierCreateRequest
	{
		public string TypeCode { get; set; }

		public Guid? ParentUid { get; set; }
	}
}
