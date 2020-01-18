using MediatR;
using Montr.Core.Models;
using Montr.Docs.Models;

namespace Montr.Docs.Queries
{
	public class GetProcessList : ProcessSearchRequest, IRequest<SearchResult<Process>>
	{
	}
}
