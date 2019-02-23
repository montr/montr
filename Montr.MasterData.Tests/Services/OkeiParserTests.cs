﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Tests.Services
{
	[TestClass]
	public class OkeiParserTests
	{
		[TestMethod]
		public async Task Parser_Should_ParseRealFile()
		{
			// arrange
			var path = "../../../Content/nsiOkei_all_20190217_022439_001.xml";
			path = "../../../Content/nsiOkved2_all_20190217_022436_001.xml";
			var parser = new OkeiParser();

			// act
			ICollection<Classifier> result;
			using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
			{
				result = await parser.Parse(stream, CancellationToken.None);
			}

			// assert
			Assert.IsNotNull(result);
			// Assert.AreEqual(583, result.Count);
			// Assert.AreEqual("728", result.Single(x => x.Name == "Пачка").Code);


			//
			//
			// temp insert all...
			//
			//

			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var dateTimeProvider = new DefaultDateTimeProvider();

			var handler = new InsertClassifierHandler(unitOfWorkFactory, dbContextFactory, dateTimeProvider);

			foreach (var classifier in result)
			{
				classifier.ConfigCode = "okei";
				classifier.ConfigCode = "okved2";

				var command = new InsertClassifier
				{
					UserUid = Guid.NewGuid(),
					CompanyUid = Guid.Parse("6465dd4c-8664-4433-ba6a-14effd40ebed"),
					Item = classifier
				};

				await handler.Handle(command, CancellationToken.None);
			}
		}
	}
}
