using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Docs.Models;
using Montr.Docs.Queries;

namespace Montr.Docs.Impl.QueryHandlers
{
	public class GetProcessListHandler : IRequestHandler<GetProcessList, SearchResult<Process>>
	{
		public Task<SearchResult<Process>> Handle(GetProcessList request, CancellationToken cancellationToken)
		{
			return Task.FromResult(new SearchResult<Process>
			{
				Rows = new[]
				{
					new Process
					{
						Uid = Process.Registration,
						Code = "Registration",
						Name = "Процесс регистрации (по умолчанию)",
						Url = $"/processes/edit/{Process.Registration}"
					}
				}
			});
		}
	}
}
