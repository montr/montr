using System;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Montr.Worker.Hangfire.Services;
using NUnit.Framework;

namespace Montr.Worker.Hangfire.Tests;

[SetUpFixture]
public class Initialize
{
	[OneTimeSetUp]
	public static void AssemblyInitialize()
	{
		var configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			// .AddJsonFile(jsonPath, true)
			// .AddUserSecrets(...)
			.AddEnvironmentVariables()
			.Build();

		GlobalConfiguration.Configuration.UseDefaults(configuration);
	}

	[OneTimeTearDown]
	public static void AssemblyCleanup()
	{
	}
}
