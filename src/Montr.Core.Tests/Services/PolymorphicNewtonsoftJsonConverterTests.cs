using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	// todo: write tests for both converters
	public class PolymorphicNewtonsoftJsonConverterTests
	{
		[TestMethod]
		public void CanConvert_InExactMode_CheckExactType()
		{
			// arrange
			var converter = new PolymorphicNewtonsoftJsonConverter<Base>("", new Dictionary<string, Type>());

			// act && assert
			Assert.AreEqual(true, converter.CanConvert(typeof(Base)));
			Assert.AreEqual(false, converter.CanConvert(typeof(Inheritor)));
			Assert.AreEqual(false, converter.CanConvert(typeof(string)));
		}

		/*[TestMethod]
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
		}

		private class Inheritor : Base
		{
		}
	}
}
