using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierList : IRequest<SearchResult<Classifier>>
	{
		public ClassifierSearchRequest Request { get; set; }
	}
}
