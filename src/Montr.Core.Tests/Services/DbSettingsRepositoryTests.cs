using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Impl.Entities;
using Montr.Core.Impl.Services;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Tests.Services
{
	[TestClass]
	public class DbSettingsRepositoryTests
	{
		[TestMethod]
		public async Task Update_WithGetOptions_ShouldInsertAndUpdate()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var repository = new DbSettingsRepository(unitOfWorkFactory, dbContextFactory);

			using (var _ = unitOfWorkFactory.Create())
			{
				// act (insert)
				var options =  repository.GetSettings<TestOptions>()
					.Set(x => x.Number, 42)
					.Set(x => x.Value, null)
					.Set(x => x.State, AppState.Initialized);

				var affected = await options.Update(cancellationToken);

				// assert (insert)
				Assert.AreEqual(3, affected);

				var allSettings = await LoadSettings(dbContextFactory, cancellationToken);

				var s1 = allSettings.SingleOrDefault(x => x.Id == "Montr.Core.Tests.Services.TestOptions.Number");
				var s2 = allSettings.SingleOrDefault(x => x.Id == "Montr.Core.Tests.Services.TestOptions.Value");
				var s3 = allSettings.SingleOrDefault(x => x.Id == "Montr.Core.Tests.Services.TestOptions.State");

				Assert.IsNotNull(s1);
				Assert.AreEqual("42", s1.Value);

				Assert.IsNotNull(s2);
				Assert.AreEqual(null, s2.Value);

				Assert.IsNotNull(s3);
				Assert.AreEqual("Initialized", s3.Value);

				// act (update)
				options =  repository.GetSettings<TestOptions>()
					.Set(x => x.State, AppState.None);

				affected = await options.Update(cancellationToken);

				// assert (update)
				Assert.AreEqual(1, affected);

				allSettings = await LoadSettings(dbContextFactory, cancellationToken);

				s3 = allSettings.SingleOrDefault(x => x.Id == "Montr.Core.Tests.Services.TestOptions.State");

				Assert.IsNotNull(s3);
				Assert.AreEqual("None", s3.Value);
			}
		}

		private static async Task<IList<DbSettings>> LoadSettings(IDbContextFactory dbContextFactory, CancellationToken cancellationToken)
		{
			await using (var db = dbContextFactory.Create())
			{
				return await db.GetTable<DbSettings>().ToListAsync(cancellationToken);
			}
		}
	}

	// ReSharper disable once ClassNeverInstantiated.Local
	internal class TestOptions
	{
		public int Number { get; set; }

		public string Value { get; set; }

		public AppState State { get; set; }
	}
}
