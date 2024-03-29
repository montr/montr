﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Commands;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;
using Montr.Core.Services.CommandHandlers;
using Montr.Core.Services.Implementations;
using Montr.Core.Services.QueryHandlers;
using NUnit.Framework;

namespace Montr.Core.Tests.Services
{
	public class CoreDbGenerator
	{
		private readonly GetEntityStatusListHandler _getEntityStatusListHandler;
		private readonly InsertEntityStatusHandler _insertEntityStatusHandler;

		public CoreDbGenerator(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			var entityStatusRepository = new DbEntityStatusRepository(dbContextFactory);

			_getEntityStatusListHandler = new GetEntityStatusListHandler(entityStatusRepository);
			_insertEntityStatusHandler = new InsertEntityStatusHandler(unitOfWorkFactory, dbContextFactory);
		}

		public string EntityTypeCode { get; set; } = "test_entity";

		public Guid EntityUid { get; set; } = Guid.NewGuid();

		public async Task<ApiResult> InsertEntityStatus(EntityStatus status, CancellationToken cancellationToken)
		{
			var result = await _insertEntityStatusHandler.Handle(new InsertEntityStatus
			{
				EntityTypeCode = EntityTypeCode,
				EntityUid = EntityUid,
				Item = status
			}, cancellationToken);

			Assert.That(result, Is.Not.Null);
			Assert.That(result.Success, Is.EqualTo(true));

			return result;
		}

		public async Task<SearchResult<EntityStatus>> GetEntityStatuses(CancellationToken cancellationToken)
		{
			var result = await _getEntityStatusListHandler.Handle(new GetEntityStatusList
			{
				EntityTypeCode = EntityTypeCode,
				EntityUid = EntityUid,
				SkipPaging = true
			}, cancellationToken);

			Assert.That(result, Is.Not.Null);

			return result;
		}
	}
}
