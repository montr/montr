using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries
{
	public class GetEntityStatus : EntityStatusSearchRequest, IRequest<EntityStatus>
	{
	}
}
