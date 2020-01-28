using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Impl.QueryHandlers
{
	public class ExportLocaleStringListHandler : IRequestHandler<ExportLocaleStringList, FileResult>
	{
		private readonly IRepository<LocaleString> _repository;
		private readonly LocaleStringSerializer _serializer;

		public ExportLocaleStringListHandler(IRepository<LocaleString> repository, LocaleStringSerializer serializer)
		{
			_repository = repository;
			_serializer = serializer;
		}

		public async Task<FileResult> Handle(ExportLocaleStringList request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			request.PageSize = Paging.MaxPageSize; // disable paging

			var searchResult = await _repository.Search(request, cancellationToken);

			var data = searchResult.Rows.OrderBy(x => x.Locale).ThenBy(x => x.Module).ThenBy(x => x.Key);

			var lists = new List<LocaleStringList>();

			foreach (var localeGroup in data.GroupBy(x => x.Locale))
			{
				var locale = new LocaleStringList
				{
					Locale = localeGroup.Key,
					Modules = new List<LocaleModuleStringList>()
				};

				foreach (var moduleGroup in localeGroup.GroupBy(x => x.Module))
				{
					locale.Modules.Add(new LocaleModuleStringList
					{
						Module = moduleGroup.Key,
						Items = moduleGroup.ToList()
					});
				}

				lists.Add(locale);
			}

			return new FileResult
			{
				ContentType = "application/json",
				FileName = $"locale-strings-{request.Locale}-{request.Module}-{DateTime.Now.ToString("u").Replace(':', '-').Replace(' ', '-')}.json",
				Stream = await _serializer.Serialize(lists, cancellationToken)
			};
		}
	}
}
