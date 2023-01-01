using System;
using Montr.Core.Models;

namespace Montr.Tasks.Models
{
	public class TaskSearchRequest : SearchRequest
	{
		public string[] StatusCodes { get; set; }

		public Guid? TaskTypeUid { get; set; }

		public Guid? Uid { get; set; }
	}
}
