﻿using Montr.Core.Services.Impl;
using NUnit.Framework;

namespace Montr.Core.Tests
{
	[SetUpFixture]
	public class Initialize
	{
		[OneTimeSetUp]
		public static void AssemblyInitialize()
		{
			DbConfigurationExtensions.SetLinq2DbTestSettings();
		}

		[OneTimeTearDown]
		public static void AssemblyCleanup()
		{
		}
	}
}
