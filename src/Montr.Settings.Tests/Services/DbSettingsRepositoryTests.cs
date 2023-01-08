﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.Settings.Entities;
using Montr.Settings.Services;
using Montr.Settings.Services.Implementations;
using Moq;
using NUnit.Framework;

namespace Montr.Settings.Tests.Services
{
	[TestFixture]
	public class DbSettingsRepositoryTests
	{
		[Test]
		public async Task Update_WithGetOptions_ShouldInsertAndUpdate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();
			var mediatorMock = new Mock<IPublisher>();
			var repository = new DbSettingsRepository(dbContextFactory, dateTimeProvider, mediatorMock.Object);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act (insert)
				var options =  repository.GetApplicationSettings<TestOptions>()
					.Set(x => x.Number, 42)
					.Set(x => x.Value, null)
					.Set(x => x.State, AppState.Initialized);

				var result = await options.Update(cancellationToken);

				// assert (insert)
				Assert.IsTrue(result.Success);
				Assert.AreEqual(3, result.AffectedRows);

				var allSettings = await LoadSettings(dbContextFactory, cancellationToken);

				var s1 = allSettings.SingleOrDefault(x => x.Key == "Montr.Settings.Tests.Services.TestOptions:Number");
				var s2 = allSettings.SingleOrDefault(x => x.Key == "Montr.Settings.Tests.Services.TestOptions:Value");
				var s3 = allSettings.SingleOrDefault(x => x.Key == "Montr.Settings.Tests.Services.TestOptions:State");

				Assert.IsNotNull(s1);
				Assert.AreEqual("42", s1.Value);

				Assert.IsNotNull(s2);
				Assert.AreEqual(null, s2.Value);

				Assert.IsNotNull(s3);
				Assert.AreEqual("Initialized", s3.Value);

				// act (update)
				options =  repository.GetApplicationSettings<TestOptions>()
					.Set(x => x.State, AppState.None);

				result = await options.Update(cancellationToken);

				// assert (update)
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);

				allSettings = await LoadSettings(dbContextFactory, cancellationToken);

				s3 = allSettings.SingleOrDefault(x => x.Key == "Montr.Settings.Tests.Services.TestOptions:State");

				Assert.IsNotNull(s3);
				Assert.AreEqual("None", s3.Value);
			}
		}

		private static async Task<IList<DbSettings>> LoadSettings(IDbContextFactory dbContextFactory, CancellationToken cancellationToken)
		{
			await using (var db = dbContextFactory.Create())
			{
				return await db.GetTable<DbSettings>()
					.Where(x => x.EntityTypeCode == Application.EntityTypeCode &&
					            x.EntityUid == Application.EntityUid)
					.ToListAsync(cancellationToken);
			}
		}
	}

	// ReSharper disable once ClassNeverInstantiated.Local
	// ReSharper disable once ClassNeverInstantiated.Global
	internal class TestOptions
	{
		public int Number { get; set; }

		public string Value { get; set; }

		public AppState State { get; set; }
	}
}
