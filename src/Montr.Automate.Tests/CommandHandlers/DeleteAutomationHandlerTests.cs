using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Montr.Automate.Commands;
using Montr.Automate.Impl.CommandHandlers;
using Montr.Automate.Impl.Models;
using Montr.Automate.Impl.Services;
using Montr.Automate.Models;
using Montr.Automate.Tests.Services;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Automate.Tests.CommandHandlers
{
	[TestClass]
	public class DeleteAutomationHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_DeleteAutomation()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var jsonSerializer = new NewtonsoftJsonSerializer();
			var automationService = new DefaultAutomationService(dbContextFactory, jsonSerializer);
			var generator = new AutomateDbGenerator(dbContextFactory);

			var handler = new DeleteAutomationHandler(unitOfWorkFactory, automationService);

			using (var _ = unitOfWorkFactory.Create())
			{
				// arrange
				var automation = new Automation
				{
					Conditions = new List<AutomationCondition>
					{
						new FieldAutomationCondition
						{
							Props = new FieldAutomationCondition.Properties
							{
								Field = "Status",
								Operator = AutomationConditionOperator.Equal,
								Value = "Published"
							}
						}
					},
					Actions = new List<AutomationAction>
					{
						new NotifyByEmailAutomationAction
						{

							Props = new NotifyByEmailAutomationAction.Properties
							{
								Recipient = "operator",
								Subject = "Test message #1",
								Body = "Hello"
							}
						}
					}
				};

				await generator.InsertAutomation(automation, cancellationToken);

				// act
				var command = new DeleteAutomation
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityTypeUid = generator.EntityTypeUid,
					Uids = new [] { automation.Uid  }
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);

				await Assert.ThrowsExceptionAsync<InvalidOperationException>(
					() => generator.GetAutomation(automation.Uid, cancellationToken));
			}
		}
	}
}
