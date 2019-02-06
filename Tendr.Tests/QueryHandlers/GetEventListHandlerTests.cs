﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Data.Linq2Db;
using Tendr.Implementation.QueryHandlers;
using Tendr.Models;
using Tendr.Queries;

namespace Tendr.Tests.QueryHandlers
{
	[TestClass]
	public class GetEventListHandlerTests
	{
		[TestMethod]
		public async Task GetEventList_Should_ReturnEventList()
		{
			// arrange
			var dbContextFactory = new DefaultDbContextFactory();

			var handler = new GetEventListHandler(dbContextFactory);

			// act
			var command = new GetEventList
			{
				UserUid = Guid.NewGuid(),
				Request = new EventSearchRequest
				{
				}
			};

			var result = await handler.Handle(command, CancellationToken.None);

			// assert
			Assert.IsNotNull(result);
			Assert.IsNotNull(result.Rows);
		}
	}
}
