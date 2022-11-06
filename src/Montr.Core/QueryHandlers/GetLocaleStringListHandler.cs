using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.QueryHandlers
{
	public class GetLocaleStringListHandler : IRequestHandler<GetLocaleStringList, SearchResult<LocaleString>>
	{
		private readonly IRepository<LocaleString> _repository;

		public GetLocaleStringListHandler(IRepository<LocaleString> repository)
		{
			_repository = repository;
		}

		public async Task<SearchResult<LocaleString>> Handle(GetLocaleStringList request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			return await _repository.Search(request, cancellationToken);
		}
	}
}
