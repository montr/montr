using MediatR;
using Tendr.Models;

namespace Tendr.Queries
{
	public class GetEvent : IRequest<Event>
	{
		public long EventId { get; set; }
	}
}