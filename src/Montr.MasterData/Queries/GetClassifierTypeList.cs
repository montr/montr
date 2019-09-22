using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierTypeList : ClassifierTypeSearchRequest, IRequest<SearchResult<ClassifierType>>
	{
	}
}
