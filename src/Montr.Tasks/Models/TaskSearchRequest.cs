using System;
using Montr.Core.Models;

namespace Montr.Tasks.Models
{
	public class TaskSearchRequest : SearchRequest
	{
		public Guid? Uid { get; set; }
	}
}
