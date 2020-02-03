using System;
using Montr.Core.Models;

namespace Montr.Messages.Models
{
	public class MessageTemplateSearchRequest : SearchRequest
	{
		public Guid? Uid { get; set; }

		public string Code { get; set; }
	}
}
