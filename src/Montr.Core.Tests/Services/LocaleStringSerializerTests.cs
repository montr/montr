using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services.Implementations;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	public class LocaleStringSerializerTests
	{
		[Test]
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
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Single(x => x.Locale == "en"), Is.Not.Null);
			Assert.That(result.Single(x => x.Locale == "en").Modules.Single(x => x.Module == "common"), Is.Not.Null);
			Assert.That(result.Single(x => x.Locale == "en").Modules.Single(x => x.Module == "common").Items.Count, Is.EqualTo(3));
		}
	}
}
