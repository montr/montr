using System.Collections.Generic;
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

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierGroupListHandler : IRequestHandler<GetClassifierGroupList, IEnumerable<ClassifierGroup>>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public GetClassifierGroupListHandler(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<IEnumerable<ClassifierGroup>> Handle(GetClassifierGroupList command, CancellationToken cancellationToken)
		{
			var request = command.Request;

			using (var db = _dbContextFactory.Create())
			{
				var all = from g in db.GetTable<DbClassifierGroup>()
					join tree in db.GetTable<DbClassifierTree>() on g.TreeUid equals tree.Uid
					join type in db.GetTable<DbClassifierType>() on tree.TypeUid equals type.Uid
					where g.CompanyUid == request.CompanyUid &&
						type.Code == request.TypeCode &&
						tree.Code == request.TreeCode
					select g;

				var data = await all
					// .Apply(request, x => x.Code)
					.Select(x => new ClassifierGroup
					{
						// Uid = x.Uid,
						Code = x.Code,
						// ParentCode = x.ParentCode,
						Name = x.Name,
						// Url = $"/classifiers/{x.ConfigCode}/edit/{x.Uid}"
					})
					.ToListAsync(cancellationToken);

				return data;
			}
		}
	}
}
