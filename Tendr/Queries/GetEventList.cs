using System;
using MediatR;
using Montr.Core.Models;
using Tendr.Models;

namespace Tendr.Queries
{
	public class GetEventList : IRequest<SearchResult<Event>>
	{
		public Guid UserUid { get; set; }

		public EventSearchRequest Request { get; set; }
	}
}
