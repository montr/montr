using MediatR;
using Montr.Tendr.Models;

namespace Montr.Tendr.Queries
{
	public class GetEvent : IRequest<Event>
	{
		public long EventId { get; set; }
	}
}
