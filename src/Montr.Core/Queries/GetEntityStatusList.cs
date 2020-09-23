using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries
{
	public class GetEntityStatusList : EntityStatusSearchRequest, IRequest<SearchResult<EntityStatus>>
	{
	}
}
