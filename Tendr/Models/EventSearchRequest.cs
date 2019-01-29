using Montr.Core.Models;

namespace Tendr.Models
{
	public class EventSearchRequest : Paging
    {
        public string Name { get; set; }
    }
}
