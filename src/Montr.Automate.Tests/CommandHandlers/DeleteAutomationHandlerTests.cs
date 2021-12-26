using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Commands;
using Montr.Automate.Impl.CommandHandlers;
using Montr.Automate.Impl.Models;
using Montr.Automate.Impl.Services;
using Montr.Automate.Models;
using Montr.Automate.Tests.Services;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using NUnit.Framework;

namespace Montr.Automate.Tests.CommandHandlers
{
	[TestFixture]
	public class DeleteAutomationHandlerTests
	{
		[Test]
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
					EntityUid = generator.EntityTypeUid,
					Uids = new [] { automation.Uid  }
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);

				Assert.ThrowsAsync<InvalidOperationException>(
					() => generator.GetAutomation(automation.Uid, cancellationToken));
			}
		}
	}
}
