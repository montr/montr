using MediatR;
using Montr.Core.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Queries
{
	public class GetEventList : EventSearchRequest, IRequest<SearchResult<Event>>
	{
	}
}
