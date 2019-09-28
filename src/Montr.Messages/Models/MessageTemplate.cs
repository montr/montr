using System;

namespace Montr.Messages.Models
{
	public class MessageTemplate
	{
		public Guid Uid { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }
	}
}
