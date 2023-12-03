using System.Linq;
using Montr.Core.Services;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	public class StringUtilsTests
	{
		[Test]
		[TestCase(null, new string[] { })]
		[TestCase("", new [] { "" })]
		[TestCase("   ", new [] { "   " })]
		[TestCase("test", new [] { "test" })]
		[TestCase("HelloWorld", new [] { "Hello", "World" })]
		[TestCase("HelloWorld!", new [] { "Hello", "World!" })]
		[TestCase("Hello World", new [] { "Hello World" })]
		[TestCase("TheLongAndWindingRoad", new [] { "The", "Long", "And", "Winding", "Road" })]
		[TestCase("ПриветМир", new [] { "Привет", "Мир" })]
		public void SplitUpperCase_ShouldReturnWordsArray(string source, string[] expected)
		{
			var actual = StringUtils.SplitUpperCase(source).ToArray();

			Assert.That(actual, Is.EqualTo(expected));
		}
	}
}
