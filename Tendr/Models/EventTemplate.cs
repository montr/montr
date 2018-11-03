namespace Tendr.Models
{
	public class EventTemplate
	{
		public System.Guid Id { get; set; }

		public EventType EventType { get; set; }

		public bool System { get; set; }

		// public bool Starred { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}
