using System;
using System.Globalization;
using Montr.Metadata.Services;
using NUnit.Framework;

namespace Montr.Metadata.Tests.Services
{
	[TestFixture]
	public class FieldProviderTests
	{
		[Test]
		[TestCase("2020-01-31 12:40:00", "2020-01-31T12:40:00")]
		public void DateFieldProvider_ReadNormalValues_ShouldDeserialize(string value, string expectedValue)
		{
			// arrange
			var expected = DateTime.Parse(expectedValue, null, DateTimeStyles.RoundtripKind);
			var provider = new DateFieldProvider();

			// act
			var result = provider.ReadInternal(value);

			// assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		[TestCase("12:45:36", "12:45:36")]
		[TestCase("23:01:15", "23:01:15")]
		[TestCase("08.01.2020 15:45:10", "15:45:10")]
		public void TimeFieldProvider_ReadNormalValues_ShouldDeserialize(string value, string expectedValue)
		{
			// arrange
			var expected = TimeSpan.Parse(expectedValue);
			var provider = new TimeFieldProvider();

			// act
			var result = provider.ReadInternal(value);

			// assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void TimeFieldProvider_Write_ShouldSerialize()
		{
			// arrange

			// act

			// assert
		}
	}
}
