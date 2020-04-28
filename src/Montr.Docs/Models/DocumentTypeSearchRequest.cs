using System;
using Montr.Core.Models;

namespace Montr.Docs.Models
{
	public class DocumentTypeSearchRequest : SearchRequest
	{
		public Guid? Uid { get; set; }

		public string Code { get; set; }
	}
}
