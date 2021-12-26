using System.Linq;
using Montr.Core.Services;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	public class QueryableExtensionsTests
	{
		[Test]
		public void OrderBy_Should_OrderAscending()
		{
			// arrange
			var source = new[]
			{
				new Item { Prop1 = 3 },
				new Item { Prop1 = 1 },
				new Item { Prop1 = 2 },
			}.AsQueryable();

			// act
			var result = source.OrderBy("prop1").ToArray();

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(1, result[0].Prop1);
			Assert.AreEqual(2, result[1].Prop1);
			Assert.AreEqual(3, result[2].Prop1);
		}

		[Test]
		public void OrderByDescending_Should_OrderDescending()
		{
			// arrange
			var source = new[]
			{
				new Item { Prop1 = 7 },
				new Item { Prop1 = 5 },
				new Item { Prop1 = 9 },
			}.AsQueryable();

			// act
			var result = source.OrderByDescending("prop1").ToArray();

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(9, result[0].Prop1);
			Assert.AreEqual(7, result[1].Prop1);
			Assert.AreEqual(5, result[2].Prop1);
		}

		[Test]
		public void OrderByAscending_ThenByDescending_Should_Order()
		{
			// arrange
			var source = new[]
			{
				new Item { Prop1 = 3, Prop2 = "str1" },
				new Item { Prop1 = 1, Prop2 = "str2" },
				new Item { Prop1 = 2, Prop2 = "str2" },
			}.AsQueryable();

			// act
			var result = source
				.OrderBy("prop2").ThenByDescending("prop1").ToArray();

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(3, result[0].Prop1);
			Assert.AreEqual("str1", result[0].Prop2);
			Assert.AreEqual(2, result[1].Prop1);
			Assert.AreEqual("str2", result[1].Prop2);
			Assert.AreEqual(1, result[2].Prop1);
			Assert.AreEqual("str2", result[2].Prop2);
		}

		[Test]
		public void OrderByDescending_ThenByAscending_Should_Order()
		{
			// arrange
			var source = new[]
			{
				new Item { Prop1 = 3, Prop2 = "str1" },
				new Item { Prop1 = 1, Prop2 = "str0" },
				new Item { Prop1 = 3, Prop2 = "str2" },
			}.AsQueryable();

			// act
			var result = source
				.OrderByDescending("prop1").ThenBy("prop2").ToArray();

			// assert
			Assert.IsNotNull(result);
			Assert.AreEqual(3, result.Length);
			Assert.AreEqual(3, result[0].Prop1);
			Assert.AreEqual("str1", result[0].Prop2);
			Assert.AreEqual(3, result[1].Prop1);
			Assert.AreEqual("str2", result[1].Prop2);
			Assert.AreEqual(1, result[2].Prop1);
			Assert.AreEqual("str0", result[2].Prop2);
		}

		private class Item
		{
			public int Prop1 { get; set; }

			public string Prop2 { get; set; }
		}
	}
}
