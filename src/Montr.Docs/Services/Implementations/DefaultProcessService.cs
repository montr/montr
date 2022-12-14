using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Entities;

namespace Montr.Docs.Services.Implementations;

public class DefaultProcessService : IProcessService
{
	private readonly IDbContextFactory _dbContextFactory;

	public DefaultProcessService(IDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory;
	}

	public async Task<ApiResult> Insert(InsertProcessStep request, CancellationToken cancellationToken)
	{
		var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

		item.Uid = Guid.NewGuid();

		int affected;

		using (var db = _dbContextFactory.Create())
		{
			affected = await db.GetTable<DbProcessStep>()
				.Value(x => x.Uid, item.Uid)
				.Value(x => x.ProcessUid, request.ProcessUid)
				.Value(x => x.TypeCode, "ui") // todo: ask user
				.Value(x => x.Name, item.Name)
				.Value(x => x.Description, item.Description)
				.Value(x => x.DisplayOrder, item.DisplayOrder)
				.InsertAsync(cancellationToken);
		}

		return new ApiResult { Uid = item.Uid, AffectedRows = affected };
	}

	public async Task<ApiResult> Update(UpdateProcessStep request, CancellationToken cancellationToken)
	{
		var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

		int affected;

		using (var db = _dbContextFactory.Create())
		{
			affected = await db.GetTable<DbProcessStep>()
				.Where(x => x.ProcessUid == request.ProcessUid &&
				            x.Uid == item.Uid)
				.Set(x => x.Name, item.Name)
				.Set(x => x.Description, item.Description)
				.Set(x => x.DisplayOrder, item.DisplayOrder)
				.UpdateAsync(cancellationToken);
		}

		return new ApiResult { AffectedRows = affected };
	}

	public async Task<ApiResult> Delete(DeleteProcessStep request, CancellationToken cancellationToken)
	{
		int affected;

		using (var db = _dbContextFactory.Create())
		{
			affected = await db.GetTable<DbProcessStep>()
				.Where(x => x.ProcessUid == request.ProcessUid &&
				            request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);
		}

		return new ApiResult { AffectedRows = affected };
	}
}
