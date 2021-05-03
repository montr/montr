using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Queries
{
	public class GetRolePermissions : SearchRequest, IRequest<SearchResult<Permission>>
	{
		public Guid RoleUid { get; set; }
	}
}
