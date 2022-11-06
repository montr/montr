using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;

namespace Montr.Core.Services.QueryHandlers
{
	public class GetAllLocaleStringsHandler : IRequestHandler<GetAllLocaleStrings, IDictionary<string, string>>
	{
		private readonly IRepository<LocaleString> _repository;

		public GetAllLocaleStringsHandler(IRepository<LocaleString> repository)
		{
			_repository = repository;
		}

		// todo: add caching or use ILocalizer
		public async Task<IDictionary<string, string>> Handle(GetAllLocaleStrings request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			var searchRequest = new LocaleStringSearchRequest
			{
				Locale = request.Locale,
				Module = request.Module,
				SkipPaging = true
			};

			var searchResult = await _repository.Search(searchRequest, cancellationToken);

			return searchResult.Rows.ToDictionary(row => row.Key, row => row.Value);
		}
	}
}
