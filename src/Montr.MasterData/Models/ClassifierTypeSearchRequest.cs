using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierTypeSearchRequest : SearchRequest
	{
		public Guid UserUid { get; set; }

		public Guid? Uid { get; set; }

		public string Code { get; set; }
	}
}
