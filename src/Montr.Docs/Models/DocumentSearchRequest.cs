using System;
using Montr.Core.Models;

namespace Montr.Docs.Models
{
	public class DocumentSearchRequest : SearchRequest
	{
		public bool FilterByUser { get; set; }

		public Guid? UserUid { get; set; }

		public Guid? Uid { get; set; }

		// todo: remove (?) - form should be loaded only for document's form tab
		public bool IncludeFields { get; set; }
	}
}
