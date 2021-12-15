using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries;

public class GetEntityRelationList : EntityRelationSearchRequest, IRequest<IList<EntityRelation>>
{
}
