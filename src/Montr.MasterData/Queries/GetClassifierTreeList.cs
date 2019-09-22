using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierTreeList : IRequest<SearchResult<ClassifierTree>>
	{
		public ClassifierTreeSearchRequest Request { get; set; }
	}
}
