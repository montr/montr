using System.Collections.Generic;

namespace Montr.MasterData.Models
{
	public class ParseResult
	{
		public ICollection<ClassifierGroup> Groups { get; set; }

		public ICollection<Classifier> Items { get; set; }

		public ICollection<ClassifierLink> Links { get; set; }
	}
}
