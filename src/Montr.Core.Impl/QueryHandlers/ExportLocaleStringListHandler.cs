using System;
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

			request.PageSize = 0; // disable paging

			var searchResult = await _repository.Search(request, cancellationToken);

			var lists = new[]
			{
				new LocaleStringList
				{
					Locale = request.Locale,
					Modules = new[]
					{
						new LocaleModuleStringList
						{
							Module = request.Module,
							Items = searchResult.Rows
						}
					}
				}
			};

			return new FileResult
			{
				ContentType = "application/json",
				FileName = $"locale-strings-{request.Locale}-{request.Module}-{DateTime.Now.ToString("u").Replace(':', '-').Replace(' ', '-')}.json",
				Stream = await _serializer.Serialize(lists, cancellationToken)
			};
		}
	}
}
