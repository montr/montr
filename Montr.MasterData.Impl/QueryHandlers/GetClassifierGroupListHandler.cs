using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierGroupListHandler : IRequestHandler<GetClassifierGroupList, ICollection<ClassifierGroup>>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public GetClassifierGroupListHandler(IDbContextFactory dbContextFactory, IClassifierTypeService classifierTypeService)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ICollection<ClassifierGroup>> Handle(GetClassifierGroupList command, CancellationToken cancellationToken)
		{
			var request = command.Request;

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			if (type.HierarchyType == HierarchyType.Groups)
			{
				using (var db = _dbContextFactory.Create())
				{
					IQueryable<DbClassifierGroup> query;

					if (request.ParentCode == null)
					{
						query = from tree in db.GetTable<DbClassifierTree>()
							join item in db.GetTable<DbClassifierGroup>() on tree.Uid equals item.TreeUid
							where tree.TypeUid == type.Uid
								&& tree.Code == request.TreeCode
								&& item.ParentUid == null
							select item;
					}
					else
					{
						query = from tree in db.GetTable<DbClassifierTree>()
							join item in db.GetTable<DbClassifierGroup>() on tree.Uid equals item.TreeUid
							join parent in db.GetTable<DbClassifierGroup>() on item.ParentUid equals parent.Uid
							where tree.TypeUid == type.Uid
								&& tree.Code == request.TreeCode
								&& parent.Code == request.ParentCode
							select item;
					}

					return query
						.OrderBy(x => x.Code).ThenBy(x => x.Name)
						.Take(Paging.MaxPageSize)
						.Select(x => new ClassifierGroup
						{
							Code = x.Code,
							Name = x.Name
						})
						.ToList();
				}
			}

			if (type.HierarchyType == HierarchyType.Items)
			{
				using (var db = _dbContextFactory.Create())
				{
					IQueryable<DbClassifier> query;

					if (request.ParentCode == null)
					{
						query = from item in db.GetTable<DbClassifier>()
							where item.TypeUid == type.Uid
								&& item.ParentUid == null
							orderby item.StatusCode, item.Code, item.Name
							select item;
					}
					else
					{
						query = from item in db.GetTable<DbClassifier>()
							join parent in db.GetTable<DbClassifier>() on item.ParentUid equals parent.Uid
							where item.TypeUid == type.Uid
								&& parent.Code == request.ParentCode
							select item;
					}

					return query
						.OrderBy(x => x.StatusCode).ThenBy(x => x.Code).ThenBy(x => x.Name)
						.Take(Paging.MaxPageSize)
						.Select(x => new ClassifierGroup
						{
							Code = x.Code,
							Name = x.Name
						})
						.ToList();
				}
			}

			return ImmutableList<ClassifierGroup>.Empty;
		}
	}
}
