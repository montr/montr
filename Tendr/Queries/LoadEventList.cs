using MediatR;
using Montr.Metadata.Models;
using Tendr.Models;

namespace Tendr.Queries
{
	public class LoadEventList : IRequest<DataResult<Event>>
	{
		public EventSearchRequest Request { get; set; }
	}
}
