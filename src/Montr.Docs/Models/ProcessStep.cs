using System;

namespace Montr.Docs.Models
{
	public class ProcessStep
	{
		public Guid Uid { get; set; }

		public int DisplayOrder { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}