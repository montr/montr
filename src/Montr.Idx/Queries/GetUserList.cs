using System;
using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Queries
{
	public class GetUserList : UserSearchRequest, IRequest<SearchResult<User>>
	{
		public Guid CurrentUserUid { get; set; }
	}
}
