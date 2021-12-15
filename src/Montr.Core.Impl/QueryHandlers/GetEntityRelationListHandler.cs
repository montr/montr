using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Impl.QueryHandlers;

public class GetEntityRelationListHandler : IRequestHandler<GetEntityRelationList, IList<EntityRelation>>
{
	private readonly IEntityRelationService _entityRelationService;

	public GetEntityRelationListHandler(IEntityRelationService entityRelationService)
	{
		_entityRelationService = entityRelationService;
	}
		
	public async Task<IList<EntityRelation>> Handle(GetEntityRelationList request, CancellationToken cancellationToken)
	{
		return await _entityRelationService.List(request, cancellationToken);
	}
}
