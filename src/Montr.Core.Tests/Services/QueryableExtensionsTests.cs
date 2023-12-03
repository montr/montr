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
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Length, Is.EqualTo(3));
			Assert.That(result[0].Prop1, Is.EqualTo(1));
			Assert.That(result[1].Prop1, Is.EqualTo(2));
			Assert.That(result[2].Prop1, Is.EqualTo(3));
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
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Length, Is.EqualTo(3));
			Assert.That(result[0].Prop1, Is.EqualTo(9));
			Assert.That(result[1].Prop1, Is.EqualTo(7));
			Assert.That(result[2].Prop1, Is.EqualTo(5));
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
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Length, Is.EqualTo(3));
			Assert.That(result[0].Prop1, Is.EqualTo(3));
			Assert.That(result[0].Prop2, Is.EqualTo("str1"));
			Assert.That(result[1].Prop1, Is.EqualTo(2));
			Assert.That(result[1].Prop2, Is.EqualTo("str2"));
			Assert.That(result[2].Prop1, Is.EqualTo(1));
			Assert.That(result[2].Prop2, Is.EqualTo("str2"));
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
			Assert.That(result, Is.Not.Null);
			Assert.That(result.Length, Is.EqualTo(3));
			Assert.That(result[0].Prop1, Is.EqualTo(3));
			Assert.That(result[0].Prop2, Is.EqualTo("str1"));
			Assert.That(result[1].Prop1, Is.EqualTo(3));
			Assert.That(result[1].Prop2, Is.EqualTo("str2"));
			Assert.That(result[2].Prop1, Is.EqualTo(1));
			Assert.That(result[2].Prop2, Is.EqualTo("str0"));
		}

		private class Item
		{
			public int Prop1 { get; set; }

			public string Prop2 { get; set; }
		}
	}
}
