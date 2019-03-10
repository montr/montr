using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierGroupListHandler : IRequestHandler<GetClassifierGroupList, ICollection<ClassifierGroup>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetClassifierGroupListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<ICollection<ClassifierGroup>> Handle(GetClassifierGroupList command, CancellationToken cancellationToken)
		{
			var request = command.Request;

			IDictionary<Guid, DbClassifierGroup> dbGroupsByUid;

			using (var db = _dbContextFactory.Create())
			{
				var all = from g in db.GetTable<DbClassifierGroup>()
					join tree in db.GetTable<DbClassifierTree>() on g.TreeUid equals tree.Uid
					join type in db.GetTable<DbClassifierType>() on tree.TypeUid equals type.Uid
					where type.CompanyUid == request.CompanyUid &&
						type.Code == request.TypeCode &&
						tree.Code == request.TreeCode
					orderby g.Code, g.Name
					select g ;

				dbGroupsByUid = await all.ToDictionaryAsync(x => x.Uid, x => x, cancellationToken);
			}

			var groupsByCode = dbGroupsByUid.Values.ToDictionary(x => x.Code, x =>
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

			foreach (var group in groupsByCode.Values.Where(x => x.ParentCode != null))
			{
				var parentGroup = groupsByCode[group.ParentCode];

				if (parentGroup.Children == null)
				{
					parentGroup.Children = new List<ClassifierGroup>();
				}

				parentGroup.Children.Add(group);
			}

			var result = groupsByCode.Values.Where(x => x.ParentCode == null).ToList();

			return result;
		}
	}
}
