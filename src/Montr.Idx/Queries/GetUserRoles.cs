using System;
using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Queries
{
	public class GetUserRoles : SearchRequest, IRequest<SearchResult<Role>>
	{
		public Guid UserUid { get; set; }
	}
}
