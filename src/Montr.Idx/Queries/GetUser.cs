using System;
using MediatR;
using Montr.Idx.Models;

namespace Montr.Idx.Queries
{
	public class GetUser : UserSearchRequest, IRequest<User>
	{
		public Guid Uid { get; set; }
	}
}
