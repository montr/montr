namespace Tendr.Models
{
	public class Event
	{
		public System.Guid Id { get; set; }

		public EventType EventType { get; set; }

		public string Number { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}