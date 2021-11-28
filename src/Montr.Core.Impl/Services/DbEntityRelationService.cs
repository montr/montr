using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services;

public class DbEntityRelationService : IEntityRelationService
{
	public Task<ApiResult> Insert(EntityRelation relation, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}

	public Task<ApiResult> Delete(EntityRelation relation, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
