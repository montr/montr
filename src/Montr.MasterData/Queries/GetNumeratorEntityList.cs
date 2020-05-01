using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetNumeratorEntityList : NumeratorEntitySearchRequest, IRequest<SearchResult<NumeratorEntity>>
	{
	}
}
