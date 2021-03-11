using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Queries
{
	public class GetRoleList : RoleSearchRequest, IRequest<SearchResult<Role>>
	{
	}
}
