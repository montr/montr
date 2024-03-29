﻿using Montr.Core.Services.Implementations;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	[TestFixture]
	// todo: write tests for both converters
	public class PolymorphicNewtonsoftJsonConverterTests
	{
		[Test]
		public void CanConvert_InExactMode_CheckExactType()
		{
			// arrange
			var converter = new PolymorphicNewtonsoftJsonConverter<Base>(x => x.TypeCode, new JsonTypeProvider<Base>());

			// act && assert
			Assert.That(converter.CanConvert(typeof(Base)), Is.EqualTo(true));
			Assert.That(converter.CanConvert(typeof(Inheritor)), Is.EqualTo(false));
			Assert.That(converter.CanConvert(typeof(string)), Is.EqualTo(false));
		}

		/*[Test]
		public void CanConvert_InInheritorsMode_AllowOnlyInheritors()
		{
			// arrange
			var converter = new PolymorphicNewtonsoftJsonConverter<Base>("", new Dictionary<string, Type>())
			{
				Mode = PolymorphicJsonConverterMode.Inheritors
			};

			// act && assert
			Assert.AreEqual(false, converter.CanConvert(typeof(Base)));
			Assert.AreEqual(true, converter.CanConvert(typeof(Inheritor)));
			Assert.AreEqual(false, converter.CanConvert(typeof(string)));
		}*/

		private class Base
		{
			// ReSharper disable once UnusedAutoPropertyAccessor.Local
			public string TypeCode { get; set; }
		}

		private class Inheritor : Base
		{
		}
	}
}
