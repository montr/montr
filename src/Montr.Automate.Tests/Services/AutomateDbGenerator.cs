using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Automate.Commands;
using Montr.Automate.Impl.CommandHandlers;
using Montr.Automate.Impl.QueryHandlers;
using Montr.Automate.Impl.Services;
using Montr.Automate.Models;
using Montr.Automate.Queries;
using Montr.Automate.Services;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Moq;

namespace Montr.Automate.Tests.Services
{
	public class AutomateDbGenerator
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly InsertAutomationHandler _insertAutomationHandler;
		private readonly GetAutomationHandler _getAutomationHandler;

		public AutomateDbGenerator(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;

			var aapfMock = new Mock<INamedServiceFactory<IAutomationActionProvider>>();
			aapfMock.Setup(x => x.Resolve(NotifyByEmailAutomationAction.TypeCode))
				.Returns(new NoopAutomationActionProvider { ActionType = typeof(NotifyByEmailAutomationAction) });
			var automationRepository = new DbAutomationRepository(dbContextFactory, aapfMock.Object, new NewtonsoftJsonSerializer());

			_insertAutomationHandler = new InsertAutomationHandler(unitOfWorkFactory, dbContextFactory);
			_getAutomationHandler = new GetAutomationHandler(automationRepository);
		}

		public string EntityTypeCode { get; set; } = "test_closure";

		public Guid EntityTypeUid { get; set; } = Guid.NewGuid();

		public async Task<ApiResult> InsertAutomation(Automation automation, CancellationToken cancellationToken)
		{
			var result = await _insertAutomationHandler.Handle(new InsertAutomation
			{
				EntityTypeCode = EntityTypeCode,
				EntityTypeUid = EntityTypeUid,
				Item = automation
			}, cancellationToken);

			Assert.IsNotNull(result);
			Assert.AreEqual(true, result.Success);

			return result;
		}

		public async Task<Automation> GetAutomation(Guid uid, CancellationToken cancellationToken)
		{
			return await _getAutomationHandler.Handle(new GetAutomation
				{
					EntityTypeCode = EntityTypeCode,
					EntityTypeUid = EntityTypeUid,
					Uid = uid
				},
				cancellationToken);
		}

		private class NoopAutomationActionProvider : IAutomationActionProvider
		{
			public Type ActionType { get; set; }

			public Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
			{
				throw new NotImplementedException();
			}
		}
	}

}
