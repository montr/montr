using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Core.Services.QueryHandlers;
using Moq;
using NUnit.Framework;

namespace Montr.Core.Tests.QueryHandlers
{
	[TestFixture]
	public class ExportLocaleStringListHandlerTests
	{
		[Test]
		public async Task ExportLocaleStringList_Should_ReturnStream()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var localeStringSerializer = new LocaleStringSerializer();

			var repository = new Mock<IRepository<LocaleString>>();
			repository
				.Setup(x => x.Search(It.IsAny<ExportLocaleStringList>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(() => new SearchResult<LocaleString>
				{
					Rows = new List<LocaleString>
					{
						new LocaleString { Locale = "en", Module = "common", Key = "key1", Value = "value1" },
						new LocaleString { Locale = "en", Module = "common", Key = "key2", Value = "value2" },
						new LocaleString { Locale = "en", Module = "common", Key = "key3", Value = "value3" },
					}
				});

			var handler = new ExportLocaleStringListHandler(repository.Object, localeStringSerializer);

			// act
			var result = await handler.Handle(new ExportLocaleStringList
			{
				Locale = "en",
				Module = "common"
			}, cancellationToken);

			// assert
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ContentType, Is.Not.Null);
			Assert.That(result.FileName, Is.Not.Null);

			using (var reader = new StreamReader(result.Stream))
			{
				var expected = await File.ReadAllTextAsync("../../../Content/locale-strings-en-common-test-1.json", cancellationToken);

				var json = (await reader.ReadToEndAsync(cancellationToken)).Replace("\r\n", "\n");

				Assert.That(json, Is.EqualTo(expected));
			}
		}
	}
}
