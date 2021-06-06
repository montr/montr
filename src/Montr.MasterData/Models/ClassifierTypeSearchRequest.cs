using System;
using Montr.Core.Models;

namespace Montr.MasterData.Models
{
	public class ClassifierTypeSearchRequest : SearchRequest
	{
		public Guid? Uid { get; set; }

		public string Code { get; set; }
	}
}
