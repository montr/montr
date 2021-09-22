using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Montr.Automate.Commands;
using Montr.Automate.Impl.Entities;
using Montr.Automate.Impl.Models;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Automate.Impl.Services
{
	public class DefaultAutomationService : IAutomationService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IJsonSerializer _jsonSerializer;

		public DefaultAutomationService(IDbContextFactory dbContextFactory, IJsonSerializer jsonSerializer)
		{
			_dbContextFactory = dbContextFactory;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<ApiResult> Insert(InsertAutomation request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			item.Uid = Guid.NewGuid();

			var dbConditions = CollectDbConditions(item);
			var dbActions = CollectDbActions(item);

			int affected;

			using (var db = _dbContextFactory.Create())
			{
				affected = await db.GetTable<DbAutomation>()
					.Value(x => x.Uid, item.Uid)
					.Value(x => x.EntityTypeCode, request.EntityTypeCode)
					.Value(x => x.EntityTypeUid, request.EntityUid)
					.Value(x => x.TypeCode, "trigger") // todo: ask user
					.Value(x => x.Name, item.Name)
					.Value(x => x.Description, item.Description)
					.Value(x => x.IsActive, true)
					.Value(x => x.IsSystem, item.System)
					.Value(x => x.DisplayOrder, item.DisplayOrder)
					.InsertAsync(cancellationToken);

				await db.GetTable<DbAutomationAction>().BulkCopyAsync(dbActions, cancellationToken);

				await db.GetTable<DbAutomationCondition>().BulkCopyAsync(dbConditions, cancellationToken);
			}

			return new ApiResult { Uid = item.Uid, AffectedRows = affected };
		}

		public async Task<ApiResult> Update(UpdateAutomation request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var dbConditions = CollectDbConditions(item);
			var dbActions = CollectDbActions(item);

			int affected;

			using (var db = _dbContextFactory.Create())
			{
				affected = await db.GetTable<DbAutomation>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode &&
								x.EntityTypeUid == request.EntityUid &&
								x.Uid == item.Uid)
					.Set(x => x.Name, item.Name)
					.Set(x => x.Description, item.Description)
					.Set(x => x.DisplayOrder, item.DisplayOrder)
					.UpdateAsync(cancellationToken);

				await db.GetTable<DbAutomationAction>()
					.Where(x => x.AutomationUid == item.Uid).DeleteAsync(cancellationToken);

				await db.GetTable<DbAutomationAction>().BulkCopyAsync(dbActions, cancellationToken);

				await db.GetTable<DbAutomationCondition>()
					.Where(x => x.AutomationUid == item.Uid).DeleteAsync(cancellationToken);

				await db.GetTable<DbAutomationCondition>().BulkCopyAsync(dbConditions, cancellationToken);
			}

			return new ApiResult { AffectedRows = affected };
		}

		public async Task<ApiResult> Delete(DeleteAutomation request, CancellationToken cancellationToken)
		{
			int affected;

			using (var db = _dbContextFactory.Create())
			{
				await db.GetTable<DbAutomationAction>()
					.Where(x => request.Uids.Contains(x.AutomationUid))
					.DeleteAsync(cancellationToken);

				await db.GetTable<DbAutomationCondition>()
					.Where(x => request.Uids.Contains(x.AutomationUid))
					.DeleteAsync(cancellationToken);

				affected = await db.GetTable<DbAutomation>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode &&
								x.EntityTypeUid == request.EntityUid &&
								request.Uids.Contains(x.Uid))
					.DeleteAsync(cancellationToken);
			}

			return new ApiResult { AffectedRows = affected };
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
