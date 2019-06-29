using Montr.Core.Models;

namespace Montr.Tendr.Models
{
	public class EventSearchRequest : Paging
    {
        public string Name { get; set; }
    }
}
