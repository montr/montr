using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierLinkListHandler : IRequestHandler<GetClassifierLinkList, SearchResult<ClassifierLink>>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public GetClassifierLinkListHandler(IDbContextFactory dbContextFactory, IClassifierTypeService classifierTypeService)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<SearchResult<ClassifierLink>> Handle(GetClassifierLinkList command, CancellationToken cancellationToken)
		{
			var request = command.Request ?? throw new ArgumentNullException(nameof(command.Request));

			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			if (type.HierarchyType != HierarchyType.Groups)
			{
				throw new InvalidOperationException("Invalid classifier hierarchy type for groups operations.");
			}

			using (var db = _dbContextFactory.Create())
			{
				IQueryable<DbClassifierLink> query = db.GetTable<DbClassifierLink>();

				if (request.GroupUid.HasValue)
				{
					query = query.Where(x => x.GroupUid == request.GroupUid);
				}

				if (request.ItemUid.HasValue)
				{
					query = query.Where(x => x.ItemUid == request.ItemUid);
				}

				var data = await query
					// todo: order by default hierarchy first, then by group name
					.Apply(request, x => x.GroupUid)
					.Select(x => new ClassifierLink
					{
						GroupUid = x.GroupUid,
						ItemUid = x.ItemUid
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<ClassifierLink>
				{
					TotalCount = query.Count(),
					Rows = data
				};
			}
		}
	}
}
