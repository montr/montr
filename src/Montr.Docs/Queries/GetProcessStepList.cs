using MediatR;
using Montr.Core.Models;
using Montr.Docs.Models;

namespace Montr.Docs.Queries
{
	public class GetProcessStepList : IRequest<SearchResult<ProcessStep>>
	{
	}
}