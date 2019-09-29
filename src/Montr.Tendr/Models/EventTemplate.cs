namespace Montr.Tendr.Models
{
    public class EventTemplate
    {
        public string ConfigCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public static readonly EventTemplate[] All =
        {
			new EventTemplate
			{
				ConfigCode = "rfi",
				Name = "Запрос информации",
				Description = "Some descriptive description"
			},
			new EventTemplate
			{
				ConfigCode = "rfp",
				Name = "Запрос предложений",
				Description = "Some descriptive description"
			},
			new EventTemplate
			{
				ConfigCode = "proposal",
				Name = "Предложение"
			}
		};
    }
}
