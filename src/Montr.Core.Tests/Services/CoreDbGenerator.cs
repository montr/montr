using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Core.Commands;
using Montr.Core.Impl.CommandHandlers;
using Montr.Core.Impl.QueryHandlers;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Queries;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

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

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

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

			Assert.IsNotNull(result);

			return result;
		}
	}
}
