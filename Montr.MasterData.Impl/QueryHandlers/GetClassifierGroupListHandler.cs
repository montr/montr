using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
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

			IDictionary<string, ClassifierGroup> groupsByCode;

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			if (type.HierarchyType == HierarchyType.Groups)
			{
				IDictionary<Guid, DbClassifierGroup> dbGroupsByUid;

				using (var db = _dbContextFactory.Create())
				{
					var all = from tree in db.GetTable<DbClassifierTree>() 
						join g in db.GetTable<DbClassifierGroup>() on tree.Uid equals g.TreeUid  
						where tree.TypeUid == type.Uid &&
							tree.Code == request.TreeCode
						orderby g.Code, g.Name
						select g;

					dbGroupsByUid = await all.ToDictionaryAsync(x => x.Uid, x => x, cancellationToken);
				}

				var sorted = DirectedAcyclicGraphVerifier.Sort(dbGroupsByUid.Values,
					node => node.Uid, node => node.ParentUid != null ? new [] { node.ParentUid } : null );

				groupsByCode = sorted.ToDictionary(x => x.Code, x =>
				{
					var group = new ClassifierGroup
					{
						Code = x.Code,
						Name = x.Name
					};

					if (x.ParentUid.HasValue)
					{
						group.ParentCode = dbGroupsByUid[x.ParentUid.Value].Code;
					}

					return group;
				});
			}
			else if (type.HierarchyType == HierarchyType.Items)
			{
				IDictionary<Guid, DbClassifier> dbItemByUid;

				using (var db = _dbContextFactory.Create())
				{
					var all =
						from parentUid in (
							from item in db.GetTable<DbClassifier>()
							where item.TypeUid == type.Uid
							select item.ParentUid).Distinct()
						join item in db.GetTable<DbClassifier>() on parentUid equals item.Uid
						select item;

					dbItemByUid = await all.ToDictionaryAsync(x => x.Uid, x => x, cancellationToken);
				}

				var sorted = DirectedAcyclicGraphVerifier.Sort(dbItemByUid.Values,
					node => node.Uid, node => node.ParentUid != null ? new [] { node.ParentUid } : null );

				groupsByCode = sorted.ToDictionary(x => x.Code, x =>
				{
					var group = new ClassifierGroup
					{
						Code = x.Code,
						Name = x.Name
					};

					if (x.ParentUid.HasValue)
					{
						group.ParentCode = dbItemByUid[x.ParentUid.Value].Code;
					}

					return group;
				});
			}
			else
			{
				groupsByCode = ImmutableDictionary<string, ClassifierGroup>.Empty;
			}
			
			BuildTree(groupsByCode);

			var result = groupsByCode.Values.Where(x => x.ParentCode == null).ToList();

			return result;
		}

		private static void BuildTree(IDictionary<string, ClassifierGroup> groupsByCode)
		{
			foreach (var group in groupsByCode.Values.Where(x => x.ParentCode != null))
			{
				var parentGroup = groupsByCode[group.ParentCode];

				if (parentGroup.Children == null)
				{
					parentGroup.Children = new List<ClassifierGroup>();
				}

				parentGroup.Children.Add(group);
			}
		}
	}
}
