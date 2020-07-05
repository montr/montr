using System;
using System.Collections.Generic;
using System.Linq;
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
	public class InsertAutomationHandlerTests
	{
		[TestMethod]
		public async Task Handle_NormalValues_InsertAutomation()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var jsonSerializer = new NewtonsoftJsonSerializer();
			var automationService = new DefaultAutomationService(dbContextFactory, jsonSerializer);
			var generator = new AutomateDbGenerator(dbContextFactory);

			var handler = new InsertAutomationHandler(unitOfWorkFactory, automationService);

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

				// act
				var command = new InsertAutomation
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityTypeUid = generator.EntityTypeUid,
					Item = automation
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);
				Assert.IsNotNull(result.Uid);
				Assert.AreNotEqual(Guid.Empty, result.Uid);

				// ReSharper disable once PossibleInvalidOperationException
				var inserted = await generator.GetAutomation(result.Uid.Value, cancellationToken);

				Assert.AreEqual(1, inserted.Actions.Count);
				var actions = inserted.Actions.OfType<NotifyByEmailAutomationAction>().ToList();
				Assert.IsNotNull(actions.FirstOrDefault(x => x.Props.Subject.Contains("#1")));

				Assert.AreEqual(1, inserted.Conditions.Count);
				var conditions = inserted.Conditions.OfType<FieldAutomationCondition>().ToList();
				Assert.IsNotNull(conditions.FirstOrDefault(x => x.Props.Field.Contains("Status")));
			}
		}
	}
}
