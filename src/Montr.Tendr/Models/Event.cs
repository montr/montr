using System;

namespace Montr.Tendr.Models
{
	public class Event
	{
		public Guid Uid { get; set; }

		public long Id { get; set; }

		// public bool System { get; set; }

		// public bool Starred { get; set; }

		public bool IsTemplate { get; set; }

		public Guid? TemplateUid { get; set; }

		// todo: remove
		public string ConfigCode { get; set; }

		public string StatusCode { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string Url { get; set; }
	}
}
