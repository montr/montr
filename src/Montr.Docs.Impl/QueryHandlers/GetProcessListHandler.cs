using System;
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
			var uid = Guid.Parse("8C41EBDC-E176-424E-9048-249E9862DBB2");

			return Task.FromResult(new SearchResult<Process>
			{
				Rows = new[]
				{
					new Process
					{
						Uid = uid,
						Code = "Registration",
						Name = "Процесс регистрации (по умолчанию)",
						Url = $"/processes/edit/{uid}"
					}
				}
			});
		}
	}
}
