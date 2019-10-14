using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;

namespace Montr.Core.Impl.QueryHandlers
{
	public class ExportLocaleStringListHandler : IRequestHandler<ExportLocaleStringList, FileResult>
	{
		private readonly IRepository<LocaleString> _repository;

		public ExportLocaleStringListHandler(IRepository<LocaleString> repository)
		{
			_repository = repository;
		}

		public async Task<FileResult> Handle(ExportLocaleStringList request, CancellationToken cancellationToken)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));

			request.PageSize = 0; // disable paging

			var searchResult = await _repository.Search(request, cancellationToken);

			var stream = new MemoryStream();

			var options = new JsonWriterOptions
			{
				Indented = true,
				Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
			};

			using (var writer = new Utf8JsonWriter(stream, options))
			{
				writer.WriteStartObject();
				writer.WriteStartObject(request.Locale);
				writer.WriteStartObject(request.Module);

				foreach (var row in searchResult.Rows)
				{
					writer.WriteString(row.Key, row.Value);
				}

				writer.WriteEndObject(); // module
				writer.WriteEndObject(); // locale
				writer.WriteEndObject();

				await writer.FlushAsync(cancellationToken);

				stream.Position = 0;

				return new FileResult
				{
					ContentType = "application/json",
					FileName = $"locale-strings-{request.Locale}-{request.Module}-{DateTime.Now.ToString("u").Replace(':', '-').Replace(' ', '-')}.json",
					Stream = stream
				};
			}
		}
	}
}
