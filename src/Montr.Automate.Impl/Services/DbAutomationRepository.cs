using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Montr.Automate.Impl.Entities;
using Montr.Automate.Impl.Models;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Services;

namespace Montr.Automate.Impl.Services
{
	public class DbAutomationRepository : DbClassifierRepository<Automation>
	{
		private readonly IJsonSerializer _jsonSerializer;
		private readonly INamedServiceFactory<IAutomationConditionProvider> _conditionProvider;
		private readonly INamedServiceFactory<IAutomationActionProvider> _actionProvider;

		public DbAutomationRepository(IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator, IJsonSerializer jsonSerializer,
			INamedServiceFactory<IAutomationConditionProvider> conditionProvider,
			INamedServiceFactory<IAutomationActionProvider> actionProvider)
			: base(dbContextFactory, classifierTypeService, classifierTreeService,
				metadataService, fieldDataRepository, numberGenerator)
		{
			_jsonSerializer = jsonSerializer;
			_conditionProvider = conditionProvider;
			_actionProvider = actionProvider;
		}

		protected override async Task<SearchResult<Classifier>> SearchInternal(DbContext db,
			ClassifierType type, ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			var classifiers = BuildQuery(db, type, request);

			var automations = db.GetTable<DbAutomation>().AsQueryable();

			var automationRequest = request as AutomationSearchRequest;

			if (automationRequest?.EntityTypeCode != null)
			{
				automations = automations.Where(x => x.EntityTypeCode == automationRequest.EntityTypeCode);
			}

			var joined = from classifier in classifiers
				join automation in automations on classifier.Uid equals automation.Uid
				select new DbItem { Classifier = classifier, Automation = automation };

			// todo: fix paging - map column to expression
			request.SortColumn ??= nameof(Classifier.Code);
			request.SortColumn = nameof(DbItem.Classifier) + "." + request.SortColumn;

			var data = await joined
				.Apply(request, x => x.Classifier.Code)
				.Select(x => Materialize(type, x))
				.Cast<Classifier>()
				.ToListAsync(cancellationToken);

			// todo: commented to load conditions and actions on classifier edit page
			// if (automationRequest?.IncludeRules == true)
			{
				foreach (var automation in data.Cast<Automation>())
				{
					// ReSharper disable once PossibleInvalidOperationException
					var automationUid = automation.Uid.Value;

					automation.Actions = await LoadActions(db, automationUid, cancellationToken);
					automation.Conditions = await LoadConditions(db, automationUid, cancellationToken);
				}
			}

			return new SearchResult<Classifier>
			{
				TotalCount = joined.GetTotalCount(request),
				Rows = data
			};
		}

		private Automation Materialize(ClassifierType type, DbItem dbItem)
		{
			var item = base.Materialize(type, dbItem.Classifier);

			var dbAutomation = dbItem.Automation;

			item.EntityTypeCode = dbAutomation.EntityTypeCode;
			item.TypeCode = dbAutomation.TypeCode;

			return item;
		}

		private async Task<List<AutomationAction>> LoadActions(DbContext db, Guid automationUid, CancellationToken cancellationToken)
		{
			var result = new List<AutomationAction>();

			var dbActions = await db.GetTable<DbAutomationAction>()
				.Where(x => x.AutomationUid == automationUid)
				.ToListAsync(cancellationToken);

			foreach (var dbAction in dbActions)
			{
				var actionProvider = _actionProvider.GetRequiredService(dbAction.TypeCode);

				// todo: use factory (?) move to provider (!?)
				var action = (AutomationAction) Activator.CreateInstance(actionProvider.RuleType.Type);

				if (action != null && dbAction.Props != null)
				{
					var propertiesType = action.GetPropertiesType();

					var properties = _jsonSerializer.Deserialize(dbAction.Props, propertiesType);

					action.SetProperties(properties);
				}

				result.Add(action);
			}

			return result;
		}

		private async Task<List<AutomationCondition>> LoadConditions(DbContext db, Guid automationUid, CancellationToken cancellationToken)
		{
			var result = new List<AutomationCondition>();

			var dbConditions = await db.GetTable<DbAutomationCondition>()
				.Where(x => x.AutomationUid == automationUid)
				.ToListAsync(cancellationToken);

			foreach (var dbCondition in dbConditions)
			{
				var conditionProvider = _conditionProvider.GetRequiredService(dbCondition.TypeCode);

				// todo: use factory (?) move to provider (!?)
				var condition = (AutomationCondition) Activator.CreateInstance(conditionProvider.RuleType.Type);

				if (condition != null && dbCondition.Props != null)
				{
					var propertiesType = condition.GetPropertiesType();

					var properties = _jsonSerializer.Deserialize(dbCondition.Props, propertiesType);

					condition.SetProperties(properties);
				}

				result.Add(condition);
			}

			return result;
		}

		protected override async Task<Automation> CreateInternal(ClassifierCreateRequest request, CancellationToken cancellationToken)
		{
			var result = await base.CreateInternal(request, cancellationToken);

			return result;
		}

		protected override async Task<ApiResult> InsertInternal(DbContext db,
			ClassifierType type, Classifier item, CancellationToken cancellationToken = default)
		{
			var automation = (Automation)item;

			var dbConditions = CollectDbConditions(automation);
			var dbActions = CollectDbActions(automation);

			var result = await base.InsertInternal(db, type, item, cancellationToken);

			if (result.Success)
			{
				await db.GetTable<DbAutomation>()
					.Value(x => x.Uid, item.Uid)
					.Value(x => x.EntityTypeCode, automation.EntityTypeCode)
					.Value(x => x.TypeCode, AutomationTypeCode.Trigger) // todo: ask user
					.InsertAsync(cancellationToken);

				await db.GetTable<DbAutomationAction>().BulkCopyAsync(dbActions, cancellationToken);

				await db.GetTable<DbAutomationCondition>().BulkCopyAsync(dbConditions, cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> UpdateInternal(DbContext db,
			ClassifierType type, ClassifierTree tree, Classifier item, CancellationToken cancellationToken = default)
		{
			var automation = (Automation)item;

			var dbConditions = CollectDbConditions(automation);
			var dbActions = CollectDbActions(automation);

			var result = await base.UpdateInternal(db, type, tree, item, cancellationToken);

			if (result.Success)
			{
				/*await db.GetTable<DbAutomation>()
					.Where(x => x.Uid == item.Uid)
					.Set(x => x.EntityTypeCode, automation.EntityTypeCode)
					.UpdateAsync(cancellationToken);*/

				await db.GetTable<DbAutomationAction>()
					.Where(x => x.AutomationUid == item.Uid).DeleteAsync(cancellationToken);

				await db.GetTable<DbAutomationAction>().BulkCopyAsync(dbActions, cancellationToken);

				await db.GetTable<DbAutomationCondition>()
					.Where(x => x.AutomationUid == item.Uid).DeleteAsync(cancellationToken);

				await db.GetTable<DbAutomationCondition>().BulkCopyAsync(dbConditions, cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> DeleteInternal(DbContext db,
			ClassifierType type, DeleteClassifier request, CancellationToken cancellationToken = default)
		{
			await db.GetTable<DbAutomationAction>()
				.Where(x => request.Uids.Contains(x.AutomationUid))
				.DeleteAsync(cancellationToken);

			await db.GetTable<DbAutomationCondition>()
				.Where(x => request.Uids.Contains(x.AutomationUid))
				.DeleteAsync(cancellationToken);

			await db.GetTable<DbAutomation>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			return await base.DeleteInternal(db, type, request, cancellationToken);
		}

		private IEnumerable<DbAutomationCondition> CollectDbConditions(Automation automation)
		{
			var result = new List<DbAutomationCondition>();

			CollectDbConditionsRecursively(result, automation.Conditions, automation.Uid.Value, null);

			return result;
		}

		private void CollectDbConditionsRecursively(
			ICollection<DbAutomationCondition> result, IList<AutomationCondition> items, Guid automationUid, Guid? parentUid)
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
							AutomationUid = automation.Uid.Value,
							TypeCode = item.Type,
							DisplayOrder = order++,
							Props = _jsonSerializer.Serialize(properties)
						});
					}
				}
			}

			return result;
		}

		private class DbItem
		{
			public DbClassifier Classifier { get; init; }

			public DbAutomation Automation { get; init; }
		}
	}
}
