using System;
using Montr.Core.Models;

namespace Montr.Docs.Models
{
	public class DocumentSearchRequest : SearchRequest
	{
		public Guid? Uid { get; set; }
	}
}
