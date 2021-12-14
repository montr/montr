﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Impl.Entities;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services;

public class DbEntityRelationService : IEntityRelationService
{
	private readonly IDbContextFactory _dbContextFactory;

	public DbEntityRelationService(IDbContextFactory dbContextFactory)
	{
		_dbContextFactory = dbContextFactory;
	}

	public async Task<ApiResult> Insert(EntityRelation relation, CancellationToken cancellationToken)
	{
		using (var db = _dbContextFactory.Create())
		{
			await db.GetTable<DbEntityRelation>()
				.Value(x => x.EntityTypeCode, relation.EntityTypeCode)
				.Value(x => x.EntityUid, relation.EntityUid)
				.Value(x => x.RelatedEntityTypeCode, relation.RelatedEntityTypeCode)
				.Value(x => x.RelatedEntityUid, relation.RelatedEntityUid)
				.Value(x => x.RelationType, relation.RelationType)
				.InsertAsync(cancellationToken);

			return new ApiResult();
		}
	}

	public async Task<ApiResult> Delete(EntityRelation relation, CancellationToken cancellationToken)
	{
		using (var db = _dbContextFactory.Create())
		{
			await db.GetTable<DbEntityRelation>()
				.Where(x => x.EntityTypeCode == relation.EntityTypeCode &&
				            x.EntityUid == relation.EntityUid &&
				            x.RelatedEntityTypeCode == relation.RelatedEntityTypeCode &&
				            x.RelatedEntityUid == relation.RelatedEntityUid &&
				            x.RelationType == relation.RelationType)
				.DeleteAsync(cancellationToken);

			return new ApiResult();
		}
	}
}