namespace Tendr.Models
{
    public class Event
    {
        public System.Guid Uid { get; set; }

 		public long Id { get; set; }

        public string ConfigCode { get; set; }

        public string StatusCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }
    }
}