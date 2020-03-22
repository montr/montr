using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class NumberPatternParserTests
	{
		[TestMethod]
		[DataRow("P-{Number}", "Number")]
		[DataRow("P-{Number}-{Year2}", "Number,Year2")]
		[DataRow("P-{Number}/{Year4}", "Number,Year4")]
		[DataRow("{Company}-{Number}/{Year4}", "Company,Number,Year4")]
		public void PatternParser_SimpleParse_ReturnTags(string pattern, string tags)
		{
			// arrange
			var parser = new NumberPatternParser();
			var expected = tags.Split(",");

			// act
			var result = parser.Parse(pattern);

			// assert
			CollectionAssert.AreEqual(expected, result.Tokens.OfType<TagToken>().Select(x => x.Name).ToArray());
		}

		[TestMethod]
		public void PatternParser_Parse_ReturnTokens_1()
		{
			// arrange
			var parser = new NumberPatternParser();

			// act
			var result = parser.Parse("P-{Company}-{Number:5}/{Year:4}");

			// assert
			Assert.AreEqual(6, result.Tokens.Count);
			AssertTextToken(result.Tokens[0], "P-");
			AssertTagToken(result.Tokens[1], "Company", new string[0]);
			AssertTextToken(result.Tokens[2], "-");
			AssertTagToken(result.Tokens[3], "Number", new[] { "5" });
			AssertTextToken(result.Tokens[4], "/");
			AssertTagToken(result.Tokens[5], "Year", new[] { "4" });
		}

		[TestMethod]
		[DataRow("{{Number}", 1, TokenType.TagBegin)]
		[DataRow("{Number}/{Year{}", 14, TokenType.TagBegin)]
		[DataRow("{Number}/{Year:{}", 15, TokenType.TagBegin)]
		[DataRow("{Number:5{}", 9, TokenType.TagBegin)]
		[DataRow("}No", 0, TokenType.TagEnd)]
		[DataRow("{}Number", 1, TokenType.TagEnd)]
		[DataRow("{Number}}", 8, TokenType.TagEnd)]
		[DataRow("{Number}/}", 9, TokenType.TagEnd)]
		[DataRow("{Number:5:}", 10, TokenType.TagEnd)]
		[DataRow("{Number:}", 8, TokenType.TagEnd)]
		[DataRow("{:Number}", 1, TokenType.TagArgSeparator)]
		[DataRow(":{:Number}", 2, TokenType.TagArgSeparator)]
		[DataRow("{Number::}", 8, TokenType.TagArgSeparator)]
		[DataRow("{Number:5}:{:}", 12, TokenType.TagArgSeparator)]
		[DataRow("{Tag1", 5, TokenType.End)]
		[DataRow("{Tag1}-{Tag2", 12, TokenType.End)]
		[DataRow("{Tag1}-{Tag2:", 13, TokenType.End)]
		public void PatternParser_ParseInvalidPattern_ThrowsException(string pattern, int errorPosition, TokenType errorToken)
		{
			// arrange
			var parser = new NumberPatternParser();

			// act
			var exception = Assert.ThrowsException<NumberPatternParseException>(() => parser.Parse(pattern));

			// assert
			Assert.AreEqual(errorPosition, exception.Position);
			Assert.AreEqual(errorToken, exception.TokenType);
		}

		private static void AssertTextToken(Token token, string content)
		{
			Assert.AreEqual(typeof(TextToken), token.GetType());
			Assert.AreEqual(content, ((TextToken)token).Content);
		}

		private static void AssertTagToken(Token token, string name, string[] args)
		{
			Assert.AreEqual(typeof(TagToken), token.GetType());
			Assert.AreEqual(name, ((TagToken)token).Name);
			CollectionAssert.AreEqual(args, ((TagToken)token).Args);
		}
	}
}
