using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.Services;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	public class LocaleStringSerializerTests
	{
		[TestMethod]
		public async Task Deserialize_Should_ReturnData()
		{
			// arrange
			var cancellationToken = new CancellationToken();

			var json = File.ReadAllText("../../../Content/locale-strings-en-common-test-1.json");
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

			var serializer = new LocaleStringSerializer();

			// act
			var result = await serializer.Deserialize(stream, cancellationToken);

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Single(x => x.Locale == "en"));
			Assert.IsNotNull(result.Single(x => x.Locale == "en").Modules.Single(x => x.Module == "common"));
			Assert.AreEqual(3, result.Single(x => x.Locale == "en").Modules.Single(x => x.Module == "common").Items.Count);
		}
	}
}
