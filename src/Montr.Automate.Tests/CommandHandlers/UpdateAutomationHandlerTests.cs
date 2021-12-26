using System.Collections.Generic;
using System.Linq;
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
	public class UpdateAutomationHandlerTests
	{
		[Test]
		public async Task Handle_NormalValues_UpdateAutomation()
		{
			// arrange
			var cancellationToken = new CancellationToken();
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var dbContextFactory = new DefaultDbContextFactory();
			var jsonSerializer = new NewtonsoftJsonSerializer();
			var automationService = new DefaultAutomationService(dbContextFactory, jsonSerializer);
			var generator = new AutomateDbGenerator(dbContextFactory);

			var handler = new UpdateAutomationHandler(unitOfWorkFactory, automationService);

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

				var inserted = await generator.InsertAutomation(automation, cancellationToken);

				// act

				// ReSharper disable once PossibleInvalidOperationException
				automation.Uid = inserted.Uid.Value;
				automation.Actions.Add(new NotifyByEmailAutomationAction
				{
					Props = new NotifyByEmailAutomationAction.Properties
					{
						Recipient = "requester",
						Subject = "Test message #2",
						Body = "Hello"
					}
				});

				var command = new UpdateAutomation
				{
					EntityTypeCode = generator.EntityTypeCode,
					EntityUid = generator.EntityTypeUid,
					Item = automation
				};

				var result = await handler.Handle(command, cancellationToken);

				// assert
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Success);
				Assert.AreEqual(1, result.AffectedRows);

				var updated = await generator.GetAutomation(inserted.Uid.Value, cancellationToken);

				Assert.AreEqual(2, updated.Actions.Count);
				var actions = updated.Actions.OfType<NotifyByEmailAutomationAction>().ToList();
				Assert.IsNotNull(actions.FirstOrDefault(x => x.Props.Subject.Contains("#1")));
				Assert.IsNotNull(actions.FirstOrDefault(x => x.Props.Subject.Contains("#2")));

				Assert.AreEqual(1, updated.Conditions.Count);
				var conditions = updated.Conditions.OfType<FieldAutomationCondition>().ToList();
				Assert.IsNotNull(conditions.FirstOrDefault(x => x.Props.Field.Contains("Status")));
			}
		}
	}
}
