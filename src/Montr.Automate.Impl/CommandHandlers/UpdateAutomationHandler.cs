using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MediatR;
using Montr.Automate.Commands;
using Montr.Automate.Impl.Entities;
using Montr.Automate.Models;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Automate.Impl.CommandHandlers
{
	public class UpdateAutomationHandler : IRequestHandler<UpdateAutomation, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IJsonSerializer _jsonSerializer;

		public UpdateAutomationHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory, IJsonSerializer jsonSerializer)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<ApiResult> Handle(UpdateAutomation request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var dbConditions = CollectDbConditions(item);
			var dbActions = CollectDbActions(item);

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					affected = await db.GetTable<DbAutomation>()
						.Where(x => x.EntityTypeCode == request.EntityTypeCode &&
									x.EntityTypeUid == request.EntityTypeUid &&
									x.Uid == item.Uid)
						.Set(x => x.Name, item.Name)
						.Set(x => x.Description, item.Description)
						.Set(x => x.DisplayOrder, item.DisplayOrder)
						.UpdateAsync(cancellationToken);

					await db.GetTable<DbAutomationCondition>()
						.Where(x => x.AutomationUid == item.Uid).DeleteAsync(cancellationToken);

					db.GetTable<DbAutomationCondition>().BulkCopy(dbConditions);

					await db.GetTable<DbAutomationAction>()
						.Where(x => x.AutomationUid == item.Uid).DeleteAsync(cancellationToken);

					db.GetTable<DbAutomationAction>().BulkCopy(dbActions);
				}

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}

		private IEnumerable<DbAutomationCondition> CollectDbConditions(Automation automation)
		{
			var result = new List<DbAutomationCondition>();

			CollectDbConditionsRecursively(result, automation.Conditions, automation.Uid, null);

			return result;
		}

		private void CollectDbConditionsRecursively(ICollection<DbAutomationCondition> result, IList<AutomationCondition> items, Guid automationUid, Guid? parentUid)
		{
			if (items != null)
			{
				var order = 0;

				foreach (var item in items)
				{
					var dbCondition = new DbAutomationCondition
					{
						Uid = Guid.NewGuid(),
						AutomationUid = automationUid,
						TypeCode = item.Type,
						DisplayOrder = order++,
						ParentUid = parentUid
					};

					var properties = item.GetProperties();

					if (properties != null)
					{
						dbCondition.Props = _jsonSerializer.Serialize(properties);
					}

					result.Add(dbCondition);

					if (item is GroupAutomationCondition groupCondition)
					{
						CollectDbConditionsRecursively(result, groupCondition.Props.Conditions, automationUid, dbCondition.Uid);
					}
				}
			}
		}

		private IEnumerable<DbAutomationAction> CollectDbActions(Automation automation)
		{
			var result = new List<DbAutomationAction>();

			if (automation.Actions != null)
			{
				var order = 0;

				foreach (var item in automation.Actions)
				{
					var properties = item.GetProperties();

					if (properties != null)
					{
						result.Add(new DbAutomationAction
						{
							Uid = Guid.NewGuid(),
							AutomationUid = automation.Uid,
							TypeCode = item.Type,
							DisplayOrder = order++,
							Props = _jsonSerializer.Serialize(properties)
						});
					}
				}
			}

			return result;
		}
	}
}
