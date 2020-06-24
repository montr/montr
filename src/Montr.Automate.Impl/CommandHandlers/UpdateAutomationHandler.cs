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

			var actions = new List<DbAutomationAction>();

			CollectDbActions(item, item.Actions, actions);

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

					await db.GetTable<DbAutomationAction>()
						.Where(x => x.AutomationUid == item.Uid).DeleteAsync(cancellationToken);

					db.GetTable<DbAutomationAction>().BulkCopy(actions);
				}

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}

		private void CollectDbActions(Automation item, IList<AutomationAction> actions, ICollection<DbAutomationAction> dbActions)
		{
			if (actions != null)
			{
				var order = 0;

				foreach (var action in actions)
				{
					var properties = action.GetProperties();

					if (properties != null)
					{
						dbActions.Add(new DbAutomationAction
						{
							AutomationUid = item.Uid,
							TypeCode = action.Type,
							DisplayOrder = order++,
							Props = _jsonSerializer.Serialize(properties)
						});
					}
				}
			}
		}
	}
}
