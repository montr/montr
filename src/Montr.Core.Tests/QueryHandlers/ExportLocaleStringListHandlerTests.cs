using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.QueryHandlers;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;
using Moq;

namespace Montr.Core.Tests.QueryHandlers
{
	[TestClass]
	public class ExportLocaleStringListHandlerTests
	{
		[TestMethod]
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
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.ContentType);
			Assert.IsNotNull(result.FileName);

			using (var reader = new StreamReader(result.Stream))
			{
				var expected = File.ReadAllText("../../../Content/locale-strings-en-common-test-1.json");

				var json = reader.ReadToEnd().Replace("\r\n", "\n");

				Assert.AreEqual(expected, json);
			}
		}
	}
}
